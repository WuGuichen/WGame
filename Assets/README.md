# WGame介绍

## 核心框架

基于Entitas的ECS模式设置角色逻辑，其余各个模块通过单例模式共享其功能，在将其余模块接入核心逻辑时可通过接口抽象减少耦合，也有直接用单例提高编写效率。

## 资源系统

基于YooAsset包实现(将示例代码集成到框架中)，可以通过配置表配置各类资源的使用或直接用资源Address名获取，提供了对象池提高效率。
</br>

[https://www.yooasset.com/](https://www.yooasset.com/)

在编辑器模式和特定模块中也会有单独的特殊资源加载模块

## UI系统

基于FariyGUI实现，可以导出C#的界面代码，BaseView用FGUI自带的Window类实现
，所有界面都看做窗口（包括全屏窗口）。子界面是树型结构，都继承同一基类以减少差异方便使用。

[https://fairygui.com/](https://fairygui.com/)

## 动作系统

主要由一个Timeline形式的编辑器编辑，数据导出为json文本并在运行时反序列化成动作数据类。

### 编辑器辅助编辑

* [动作编辑器(更统一的编辑)](Editor/MotionEditor/README.md)
* [技能编辑器(更复杂的编辑(完善中))](Editor/AbilityEditor/README.md)

### 动画

使用了基于Playable API的Animancer插件替代基础的animator。</br>
优点是更高效的替换动画片段，更方便控制blendTree和动画层级。其余额外功能还未使用到。

## AI系统

行为树、状态机、GOAP。</br>
其中行为树和状态机主要将开源项目代码用WLang特定格式编辑。</br>
GOAP则是将插件集成到AI系统中，C#实现

* [FSM](Scripts/WLang/Code/FSM/README.md)
* [BehaviorTree](Scripts/WLang/Code/BTree/README.md)
* [GOAP](Scripts/ECS/GOAPAI/README.md)

## 消息系统

### 事件系统

用委托实现事件系统。

### 通知系统（触发器事件）

为单个实体设计的更加动态的事件系统，可以编辑触发次数和触发条件，运行时可高效动态增删、检测、通知。

## 输入系统

基于InputSystem包实现。支持多设备(移动端数据直接用UI实现了)和动态绑定按键功能。

## 物理系统

由于物理系统实现难度大，只加了几种简单形状的数据结构和相交检测代码，
以及点和AABB的八叉树和BVH做空间划分，目前基本没有在使用的自定义物理检测。

## 脚本系统

由于C#也可以某种程度上热更新和热重载，项目中没有接入Lua。
而且加入了基于Antlr4实现的一个解释型的脚本语言WLang，其理念是纯文本但低代码 。</br>
优势是可以比较高效调用包装好的C#定义的方法（只多了几次遍历和字符串字典映射）,
以及更高度的定制（例如可以每个entity独占一个定义域达成类似于类的效果）等等。 </br>

[WLang介绍](Scripts/WLang/Runtime/README.md)

## 索敌系统

多对多的索敌系统，目前稳定有M*N次检查(两个阵营双方两两都更新数据，主要消耗为距离，角度等信息)