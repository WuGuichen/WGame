﻿﻿using System;
using System.Collections.Generic;
using COSXML.Model;
using COSXML.CosException;
using COSXML.Model.Object;
using System.IO;
using COSXML.Common;
using COSXML.Utils;
using COSXML.Model.Tag;
using COSXML.Log;
using System.Threading;
/**
* Copyright (c) 2018 Tencent Cloud. All rights reserved.
* 11/29/2018 4:58:32 PM
* bradyxiao
*/
namespace COSXML.Transfer
{
    public sealed class COSXMLUploadTask : COSXMLTask, OnMultipartUploadStateListener
    {
        private long divisionSize;
        private long sliceSize;

        private const int MAX_ACTIVIE_TASKS = 2;
        private volatile int activieTasks = 0;

        private long sendOffset = 0L;
        private long sendContentLength = -1L; // 实际要发送的总长度，类似于content-length
        private string srcPath;
        
        private PutObjectRequest putObjectRequest;
        private DeleteObjectRequest deleteObjectRequest;

        private Object syncExit = new Object();
        private bool isExit = false;

        private ListPartsRequest listPartsRequest;

        private InitMultipartUploadRequest initMultiUploadRequest;
        private string uploadId;

        private Dictionary<UploadPartRequest, long> uploadPartRequestMap;
        private List<UploadPartRequest> uploadPartRequestList;
        private List<SliceStruct> sliceList;
        private Object syncPartCopyCount = new object();
        private int sliceCount;
        private long hasReceiveDataLength = 0;
        private object syncProgress = new Object();

        private CompleteMultipartUploadRequest completeMultiUploadRequest;

        private AbortMultipartUploadRequest abortMultiUploadRequest;

        public COSXMLUploadTask(string bucket, string region, string key)
            : base(bucket, region, key)
        {
        }

        internal void SetDivision(long divisionSize, long sliceSize)
        {
            this.divisionSize = divisionSize;
            this.sliceSize = sliceSize;
        }

        public void SetSrcPath(string srcPath)
        {
            SetSrcPath(srcPath, -1L, -1L);
        }

        public void SetSrcPath(string srcPath, long fileOffset, long contentLength)
        {
            this.srcPath = srcPath;
            this.sendOffset = fileOffset >= 0 ? fileOffset : 0;
            this.sendContentLength = contentLength >= 0 ? contentLength : -1L;
        }

        public void SetUploadId(string uploadId)
        {
            this.uploadId = uploadId;
        }

        internal void Upload()
        {
            //UpdateTaskState(TaskState.WAITTING);
            taskState = TaskState.WAITTING;
            hasReceiveDataLength = 0;
            FileInfo fileInfo = null;
            long sourceLength = 0;
            try
            {
                fileInfo = new FileInfo(srcPath);
                sourceLength = fileInfo.Length;
            }
            catch (Exception ex)
            {
                lock (syncExit)
                {
                    if (isExit)
                    {
                        return;
                    }
                }
                if (UpdateTaskState(TaskState.FAILED))
                {
                    if (failCallback != null)
                    {
                        failCallback(new CosClientException((int)CosClientError.INVALID_ARGUMENT, ex.Message, ex), null);
                    }
                }
                //error
                return;
            }

            if(sendContentLength == -1L || (sendContentLength + sendOffset > sourceLength))
            {
                sendContentLength = sourceLength - sendOffset;
            }
            taskState = TaskState.RUNNING;
            if (sendContentLength > divisionSize)
            {
                MultiUpload();
            }
            else
            {
                SimpleUpload();
            }

        }

        private void SimpleUpload()
        {
            putObjectRequest = new PutObjectRequest(bucket, key, srcPath, sendOffset, sendContentLength);
            if (progressCallback != null)
            {
                putObjectRequest.SetCosProgressCallback(progressCallback);
            }
            cosXmlServer.PutObject(putObjectRequest, delegate(CosResult cosResult)
            {
                lock (syncExit)
                {
                    if (isExit)
                    {
                        if (taskState == TaskState.CANCEL)
                        {
                            DeleteObject();
                        }
                        return;
                    }
                }
                if (UpdateTaskState(TaskState.COMPLETED))
                {
                    PutObjectResult result = cosResult as PutObjectResult;
                    UploadTaskResult copyTaskResult = new UploadTaskResult();
                    copyTaskResult.SetResult(result);
                    if (successCallback != null)
                    {
                        successCallback(copyTaskResult);
                    }
                }
                
            },
                delegate(CosClientException clientEx, CosServerException serverEx)
                {
                    lock (syncExit)
                    {
                        if (isExit)
                        {
                            return;
                        }
                    }
                    if (UpdateTaskState(TaskState.FAILED))
                    {
                        if (failCallback != null)
                        {
                            failCallback(clientEx, serverEx);
                        }
                    }
                });
        }

        private void MultiUpload()
        {
            ComputeSliceNums();
            if (uploadId != null)
            {
                ListMultiParts();
            }
            else
            {
                InitMultiUploadPart();
            }
        }

        private void InitMultiUploadPart()
        {
            initMultiUploadRequest = new InitMultipartUploadRequest(bucket, key);
            cosXmlServer.InitMultipartUpload(initMultiUploadRequest, delegate(CosResult cosResult)
            {
                lock (syncExit)
                {
                    if (isExit)
                    {
                        return;
                    }
                }
                InitMultipartUploadResult result = cosResult as InitMultipartUploadResult;
                uploadId = result.initMultipartUpload.uploadId;
                //通知执行PartCopy
                OnInit();

            },
            delegate(CosClientException clientEx, CosServerException serverEx)
            {
                lock (syncExit)
                {
                    if (isExit)
                    {
                        return;
                    }
                }
                if (UpdateTaskState(TaskState.FAILED))
                {
                    OnFailed(clientEx, serverEx);
                }

            });
        }

        private void ListMultiParts()
        {
            listPartsRequest = new ListPartsRequest(bucket, key, uploadId);
            cosXmlServer.ListParts(listPartsRequest, delegate(CosResult cosResult)
            {
                lock (syncExit)
                {
                    if (isExit)
                    {
                        return;
                    }
                }
                ListPartsResult result = cosResult as ListPartsResult;
                //更细listParts
                UpdateSliceNums(result);
                //通知执行PartCopy
                OnInit();
               
            },
            delegate(CosClientException clientEx, CosServerException serverEx)
            {
                lock (syncExit)
                {
                    if (isExit)
                    {
                        return;
                    }
                }
                if (UpdateTaskState(TaskState.FAILED))
                {
                    OnFailed(clientEx, serverEx);
                }
            });
        }

        private void UploadPart()
        {
            activieTasks = 0;
            int size = sliceList.Count;
            sliceCount = size;
            uploadPartRequestMap = new Dictionary<UploadPartRequest, long>(size);
            uploadPartRequestList = new List<UploadPartRequest>(size);

            AutoResetEvent resetEvent = new AutoResetEvent(false);
            
            for (int i = 0; i < size; i++)
            {
                if (activieTasks > MAX_ACTIVIE_TASKS)
                {
                    resetEvent.WaitOne();
                }
                lock (syncExit)
                {
                    if (isExit)
                    {
                        return;
                    }
                }
                SliceStruct sliceStruct = sliceList[i];
                if (!sliceStruct.isAlreadyUpload)
                {
                    UploadPartRequest uploadPartRequest = new UploadPartRequest(bucket, key, sliceStruct.partNumber, uploadId, srcPath, sliceStruct.sliceStart,
                        sliceStruct.sliceLength);

                    //打印进度
                    uploadPartRequest.SetCosProgressCallback(delegate (long completed, long total)
                    {
                        lock (syncProgress)
                        {
                            long dataLen = hasReceiveDataLength + completed - uploadPartRequestMap[uploadPartRequest];
                            UpdateProgress(dataLen, sendContentLength, false);
                            hasReceiveDataLength = dataLen;
                            uploadPartRequestMap[uploadPartRequest] = completed;
                        }
                    });

                    uploadPartRequestMap.Add(uploadPartRequest, 0);
                    uploadPartRequestList.Add(uploadPartRequest);


                    Interlocked.Increment(ref activieTasks);
    
                    cosXmlServer.UploadPart(uploadPartRequest, delegate (CosResult result)
                    {
                        Interlocked.Decrement(ref activieTasks);
                        UploadPartResult uploadPartResult = result as UploadPartResult;
                        sliceStruct.eTag = uploadPartResult.eTag;
                        lock (syncPartCopyCount)
                        {
                            sliceCount--;
                            if (sliceCount == 0)
                            {
                                OnPart();
                            }
                        }
                        resetEvent.Set();

                    }, delegate (CosClientException clientEx, CosServerException serverEx)
                    {
                        Interlocked.Decrement(ref activieTasks);
                        if (UpdateTaskState(TaskState.FAILED))
                        {
                            OnFailed(clientEx, serverEx);
                        }
                        resetEvent.Set();
                    });

                }
                else
                {
                    lock (syncPartCopyCount)
                    {
                        sliceCount--;
                        if (sliceCount == 0)
                        {
                            OnPart();
                            return;
                        }
                    }
                }
            }
        }

        private void UpdateProgress(long complete, long total, bool isCompleted)
        {

            lock (syncExit)
            {
                if (isExit)
                {
                    return;
                }
            }

            if (complete < total)
            {
                if (progressCallback != null)
                {
                    progressCallback(complete, total);
                }
            }
            else
            {
                if (isCompleted)
                {
                    if (progressCallback != null)
                    {
                        progressCallback(complete, total);
                    }
                }
                else
                {
                    if (progressCallback != null)
                    {
                        progressCallback(total - 1, total);
                    }
                }
            }
            
        }

        private void CompleteMultipartUpload()
        {
            completeMultiUploadRequest = new CompleteMultipartUploadRequest(bucket, key, uploadId);
            foreach (SliceStruct sliceStruct in sliceList)
            {
                completeMultiUploadRequest.SetPartNumberAndETag(sliceStruct.partNumber, sliceStruct.eTag); // partNumberEtag 有序的
            }
            cosXmlServer.CompleteMultiUpload(completeMultiUploadRequest, delegate(CosResult result)
            {
                lock (syncExit)
                {
                    if (isExit)
                    {
                        return;
                    }
                }
                if (UpdateTaskState(TaskState.COMPLETED))
                {
                    CompleteMultipartUploadResult completeMultiUploadResult = result as CompleteMultipartUploadResult;
                    OnCompleted(completeMultiUploadResult);
                }

            }, delegate(CosClientException clientEx, CosServerException serverEx)
            {
                lock (syncExit)
                {
                    if (isExit)
                    {
                        return;
                    }
                }

                if (UpdateTaskState(TaskState.FAILED))
                {
                    OnFailed(clientEx, serverEx);
                }

            });
        } 

        private void ComputeSliceNums()
        {
            int count = (int)(sendContentLength / sliceSize);
            sliceList = new List<SliceStruct>(count > 0 ? count : 1);
            int i = 1;// partNumber >= 1
            for (; i < count; i++)
            {
                SliceStruct sliceStruct = new SliceStruct();
                sliceStruct.partNumber = i;
                sliceStruct.isAlreadyUpload = false;
                sliceStruct.sliceStart = sendOffset + (i - 1) * sliceSize;
                sliceStruct.sliceLength = sliceSize;
                sliceStruct.sliceEnd = sendOffset + i * sliceSize - 1;
                sliceList.Add(sliceStruct);
            }
            SliceStruct lastSliceStruct = new SliceStruct();
            lastSliceStruct.partNumber = i;
            lastSliceStruct.isAlreadyUpload = false;
            lastSliceStruct.sliceStart = sendOffset + (i - 1) * sliceSize;
            lastSliceStruct.sliceLength = sendContentLength  - (i - 1) * sliceSize;
            lastSliceStruct.sliceEnd = sendOffset + sendContentLength - 1;
            sliceList.Add(lastSliceStruct);
        }

        private void UpdateSliceNums(ListPartsResult listPartsResult)
        {
            try
            {
                if (listPartsResult.listParts.parts != null)
                {
                    //获取原来的parts并提取partNumber
                    Dictionary<int, SliceStruct> sourceParts = new Dictionary<int, SliceStruct>(sliceList.Count);
                    foreach (SliceStruct sliceStruct in sliceList)
                    {
                        sourceParts.Add(sliceStruct.partNumber, sliceStruct);
                    }
                    foreach (ListParts.Part part in listPartsResult.listParts.parts)
                    {
                        int partNumber = -1;
                        bool parse = int.TryParse(part.partNumber, out partNumber);
                        if (!parse) throw new ArgumentException("ListParts.Part parse error");
                        SliceStruct sliceStruct = sourceParts[partNumber];
                        sliceStruct.isAlreadyUpload = true;
                        sliceStruct.eTag = part.eTag;
                        lock (syncProgress)
                        {
                            long size = 0L;
                            long.TryParse(part.size, out size);
                            hasReceiveDataLength += size;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lock (syncExit)
                {
                    if (isExit)
                    {
                        return;
                    }
                }
                if (UpdateTaskState(TaskState.FAILED))
                {
                    OnFailed(new CosClientException((int)CosClientError.INTERNA_LERROR, ex.Message, ex), null);
                }
            }

        }

        public void OnInit()
        {
            //获取了uploadId
            UploadPart();
        }

        public void OnPart()
        {
            //获取了 part ETag
            CompleteMultipartUpload();
        }

        public void OnCompleted(CompleteMultipartUploadResult result)
        {
            UpdateProgress(sendContentLength, sendContentLength, true);
            //lock (syncExit)
            //{
            //    isExit = true;
            //}
            if (successCallback != null)
            {
                UploadTaskResult uploadTaskResult = new UploadTaskResult();
                uploadTaskResult.SetResult(result);
                successCallback(uploadTaskResult);
            }
        }

        public void OnFailed(CosClientException clientEx, CosServerException serverEx)
        {
            lock (syncExit)
            {
                isExit = true;
            }
            if (failCallback != null)
            {
                failCallback(clientEx, serverEx);
            }
        }

        private void Abort()
        {
            abortMultiUploadRequest = new AbortMultipartUploadRequest(bucket, key, uploadId);
            cosXmlServer.AbortMultiUpload(abortMultiUploadRequest, delegate (CosResult cosResult) { },
                delegate (CosClientException cosClientException, CosServerException cosServerException) { DeleteObject(); });

        }
        private void DeleteObject()
        {
            deleteObjectRequest = new DeleteObjectRequest(bucket, key);
            cosXmlServer.DeleteObject(deleteObjectRequest, delegate (CosResult cosResult) { },
                delegate (CosClientException cosClientException, CosServerException cosServerException) { });
        }

        private void RealCancle()
        {
            //cancle request
            cosXmlServer.Cancel(putObjectRequest);
            cosXmlServer.Cancel(initMultiUploadRequest);
            cosXmlServer.Cancel(completeMultiUploadRequest);
            cosXmlServer.Cancel(listPartsRequest);
            if (uploadPartRequestList != null)
            {
                foreach (UploadPartRequest uploadPartRequest in uploadPartRequestList)
                {
                    cosXmlServer.Cancel(uploadPartRequest);
                }
            }
        }

        public override void Pause()
        {
            if (UpdateTaskState(TaskState.PAUSE))
            {
                lock (syncExit) { isExit = true; }//exit upload
                //cancle request
                RealCancle();
            }
        }

        public override void Cancel()
        {
            if (UpdateTaskState(TaskState.CANCEL))
            {
                lock (syncExit) { isExit = true; }//exit upload
                //cancle request
                RealCancle();
                //abort
                Abort();
                uploadId = null;
            }
        }

        public override void Resume()
        {
            if (UpdateTaskState(TaskState.RESUME))
            {
                lock (syncExit)
                {
                    isExit = false;//continue to upload
                }
                Upload();
            }
        }

        public class UploadTaskResult : CosResult
        {
            public string eTag;

            public void SetResult(PutObjectResult result)
            {
                this.eTag = result.eTag;
                this.httpCode = result.httpCode;
                this.httpMessage = result.httpMessage;
                this.responseHeaders = result.responseHeaders;
            }

            public void SetResult(CompleteMultipartUploadResult result)
            {
                this.eTag = result.completeResult.eTag;
                this.httpCode = result.httpCode;
                this.httpMessage = result.httpMessage;
                this.responseHeaders = result.responseHeaders;
            }

            public override string GetResultInfo()
            {
                return base.GetResultInfo() + ("\n : ETag: " + eTag);
            }
        }
    }
}
