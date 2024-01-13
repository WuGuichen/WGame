using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UosCdn
{
    [Serializable]
    public class RootBuckets
    {
        public Bucket[] Buckets;
    }

    [Serializable]
    public class RootEntries
    {
        public Entry[] Entries;
    }

    [Serializable]
    public class RootReleases
    {
        public Release[] Releases;
    }

    [Serializable]
    public class RootBadges
    {
        public Badge[] Badges;
    }
}