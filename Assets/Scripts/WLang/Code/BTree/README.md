# 行为树
基于fluidBTree模块
* https://github.com/ashblue/fluid-behavior-tree

## _init.wl
导入将要使用的行为树逻辑

## 核心思想
* 用简短的代码描述最核心的逻辑
* 优先考虑通过设置状态而非直接执行行为
  + 用SetDetectState()代替DoDetect()