# GOAP
本项目的理解，更详细的可以去GOAP文档看
## Goal

### 示例
PatrolGoal
[示例](Patrol/PatrolGoal.cs) <br/>
继承GoalBase

### 作用
* 本身不维持状态信息
* 给Plannar提供信息，借此Plannar决定要执行的最合适的操作

## Action

Action由3部分组成:
* Config : action的配置
* Action Class : action的逻辑和行为
* Action Data : 用于存储临时数据

### 示例

#### Action Config

##### Conditions

是一组世界状态（每个状态或条件即一个world key, 并设置其为True或False）
，必须满足这组条件才能执行操作

##### Effects

翻译成效果，描述执行操作之后改变世界状态为预期结果

##### Base Cost

执行操作的基础cost（不包括其他额外的cost）

##### Target

每一个action都有关联一个target position,
在执行操作之前，agent将根据MoveMode向target移动.
根据targetKey来识别目标.

##### InRange

这个属性表示action执行所需的接近度（代理与目标的distance）

#### MoveMode

两种：
* **MoveBeforePerforming**: The agent moves to the target position before initiating the action.
* **PerformWhileMoving**: The agent concurrently moves to the target and executes the action.

#### Action Data

使用提示：引用其他agent类可以用[GetComponent]标签

#### Action Class
PatrolAction
[ActionClass](Patrol/PatrolAction.cs) <br/>

#### ActionRunState

* **Continue**: The action will persist and be re-evaluated in the next frame.
* **Stop**: The action will terminate, and control will revert to the planner.

## AgentBehavior

### 参数
* Current Goal: agent当前正在尝试实现的目标
* Active Action: agent当前为实现其目标而执行的操作
* WorldData: 表示游戏的当前状态，planner用它来决定agent的最佳action

### Movement

agent在执行actions前需要达到所关联的targets。由于不同游戏的Movement机制会有所不同，
这里没有特定的实现。但是这里提供了各种时间来让开发者决定agent什么时候移动。

#### MoveMode

有些actions要在moving的时候perform，所以有这个配置

#### Distance Multiplier

启发式搜索用cost表示距离，则Distance Multiplier表示速度

#### Custom Distance Calculation

通过继承IAgentDistanceObserver并赋值agent.DistanceObserver可以自定义距离计算的方式
[示例](WDistanceObservers.cs)

### Methods

#### SetGoal
```csharp
// 修改agent的当前目标，endAction表示是否终止正在进行的action
public void SetGoal<TGoal>(bool endAction) where TGoal : IGoalBase;
public void SetGoal(IGoalBase goal, bool endAction);
```

### Determining the Goal (Agent Brain)

选择最佳目标的方法是game-specific的。

### Events

AgentBehaviour提供了多个事件来用于管理agent行为和响应

可见
[WAgentActorBase示例](WAgentActorBase.cs)
[WAgentBrainBase示例](WAgentBrainBase.cs)

### GoapSet

Goals和Actions的集合

#### Goals
agent可以追求的各种目标，每个目标包含相应配置(GaolConfigs), 详细表达goal具体内容

#### Actions

Actions是agent可以执行的任务或行为。是agent试图实现goals的手段
包含ActionConfigs列表，概述agent可以执行的actions集合

#### Target Sensors

识别或指向一个Target，可以是位置，物体等等。<\br>

TargetSensorConfigs帮助agent确定它应该关注的Target
[PatrolTargetSensor示例](Patrol/PatrolTargetSensor.cs)

#### World Sensors

World Sensors 为agent提供环境信息，帮助其做出决策

### Agent Debugger Class

可以继承IAgentDebugger来在node viewer显示要显示的数据

## Sensors

Sensors帮助GOAP系统了解当前游戏情况。有两种主要类型：
* WorldSensor
* TargetSensor

### Global vs.Local Sensor

* Global: 这些sensors为所有agent提供信息。
* Local: 这些sensors仅在Planner运行时进行检查，只向一个特定agent提供信息。

### WorldSensor

用WorldKey来显示每种情况

#### 示例

[IsHateSensor](Hate/IsHateSensor.cs)

### TargetSensor

用来找到一个TargetKey的position。Planner借此了解actions的进度 
</br>
通过继承ITarget可以定义不同类型的Target，例如：[EntityTarget](EntityTarget.cs)

## TargetKeys

这些TargetKeys帮助Planner计算Actions之间的distance(and added cost), 
以及agent在执行特定action前需要达到的精确位置。
</br>
每个TargetKey 都与一个TargetSensor关联。
Sensor负责确定并提供与TargetKey对应的准确位置。

### 示例
[PatrolTarget](Patrol/PatrolTarget.cs)

## WorldKeys

### 示例
[IsPatroling](Patrol/IsPatroling.cs)