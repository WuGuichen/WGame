# WLang介绍

* 脚本主张低代码，语法不多且使用不频繁。

* 语法定义文件内可以预览代码，检验合理性和执行顺序，以下是示例和简单讲解

## 基本语法
### 文件标题(Title)
每个文件都要有的元素，其实是真正的文件名，单个文件一个标题，位于第一行
```
## 文件标题
```

### 第一个WLang文件
```
## Hello
Print("Hello World")
```

### 变量类型

```csharp
// 1.整型(INT)
a = 100  

// 2.浮点型(FLOAT)
a = 1.43 
b = 2.0

// 3.布尔型(BOOLEAN)
a = true
b = false

// 4.字符串(STRING)
a = "hello"  

// 5.列表(TABLE)
a = [1, "hi", 2.4]
b = 1:5:2               // = [1, 3, 5]列表生成式,最后一项表示间隔，默认为1
c = 1:5                 // = 1:5:1
c = [1, 1:4, c]

// 6.方法(METHOD)
a = Print                       // 方法ID
def b(p1, p2){ somecode }       // 定义式
c = (p1, p2) => { somecode }    // 匿名表达式

// 7.空类型(NIL)
a = nil
```

### 变量赋值

#### 动态类型

可以不按类型随意赋值


### 列表
可以放任何元素

```csharp
list = [GetValue(), 3.2, 4.3]
a = 3*[1,4]       // 仅右值
a = 3*list*4      // = [3*GetValue(), 3*3.2, 3*4.3], 仅支持纯数字和右值运算
// 还没做除法，可以乘以倒数

// 访问列表元素
// 当前仅支持下标访问(下标从0开始)
list = [1, 4, 5]
a = list#1    // a = 4
list#2 = 0    // 可以更改列表元素
```

### 变量运算
去语法定义文件的expr规则可以看到优先级排序, 基本的运算都有

### 概念：Block, 花括号{}包围起来的部分(包括括号)
很多语法都用到

### 条件和循环语句

```csharp
// 条件语句执行内容都用block包围
if condition > 1 + 34 and 2 > 4 or false {
    Print("If yes")
}elif someCondition {
    Print("Else if yes")
}else{
    Print("Else yes")
}
```

```csharp
// while循环
while condition{
    doSomething()
    // break 还没做break
}
// 只能遍历列表
for i in table {
    doSomthing()
    // break 还没做break
}
```

### 方法

```csharp
// 定义（默认返回值是nil)
def func(p1, p2){
    return p1
}
func = (p1, p2) => { return p1}  // 同上

// 调用(参数可以与定义的不等, 按顺序传入）, 还没试递归，正确性未知
p1 = 4
p4 = 0
a = func(p4, p1, 32)
Print(a)    // 结果是0
b = func
c = b(23, p1)
Print(c)    // 结果是23

// 一种表达方法
@E_SELF:GetValue()      // 其中@E_SELF等同E_SELF但要加@才能这样写
GetValue(E_SELF)        // 上式与下式等同
GetValue(@E_SELF)       // 这三个都一样
```

## 重要概念(编码中不需要关心)
此处先介绍WLang在游戏中的角色：  
* WLang脚本主要用于游戏核心逻辑部分
* 脚本由C#内解释器执行，游戏中有多个解释器(例如一个角色对应一个虚拟机)，不同解释器执行会控制不同数据从而达到不同的效果

### 数据域(根据类型决定GC时机，也就是有效期限)
* **代码中不需要手动管理**，主要是决定在c#端记录数据根据需求应当记录在哪里  
* 根据覆盖范围高到低以及访问优先级低到高有以下三种
* 只需大致了解
#### 全局共享数据(SharedDefinition)
* 全局函数，在MethodDefine.cs中定义
* 全局常数，在ConstDefine.cs中定义
* 该类型在游戏结束后清空数据,也可以手动释放
#### 角色缓存数据(CachedDefinition)
* 在各自角色虚拟机上定义
* 该类型在组件清除或角色销毁后清空数据,也可以手动释放
#### 基础数据域(BaseDefinition)
* 在WLang的motion脚本中定义的参数
* 该类型在每帧结束后清空数据

### 定义域
* 有全局定义域，用来定义不被销毁的公共数据，存在全局共享数据内
* 以及代码中会自动嵌套定义域来实现代码基本功能

## 特殊语法

### FSM，有限状态机
[FSM](../Code/FSM/README.md)

### BTree, 行为树
[BTree](../Code/BTree/README.md)