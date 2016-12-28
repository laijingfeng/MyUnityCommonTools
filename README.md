#### Contents
- [MyUnityCommonTools](#myunitycommontools)
    - [说明](#)
    - [简介](#)
        - [01.AssetsSettings](#01assetssettings)
        - [02.BakePrefab](#02bakeprefab)
        - [03.Collect](#03collect)
        - [04.Common](#04common)
        - [05.Drawer](#05drawer)
        - [06.ConditionalHideAttribute](#06conditionalhideattribute)
        - [07.CurveTool](#06curvetool)
        - [08.EasyFramerateCount](#07easyframeratecount)
        - [09.EnumSort](#08enumsort)
        - [10.FindReferences](#09findreferences)
        - [11.FixMask](#10fixmask)
        - [12.GestureJudge](#12gesturejudge)
        - [13.iTween](#13itween)
        - [14.LoopScroll](#14loopscroll)
        - [15.MiniMap](#15minimap)
        - [16.RadarChart](#16radarchart)
        - [17.WayPointMgr](#17waypointmgr)
        - [18.Gradient](#18gradient)
        - [19.EventMgrSys](#19eventmgrsys)
		- [20.Singleton](#20singleton)

项目 | 内容
---|---
标题 | MyUnityCommonTools
目录 | Github
标签 | Github
备注 | 无
最近更新 | 2016-12-29 01:46:48

# MyUnityCommonTools

## 说明

常用的一些Unity项目公用组件，部分参考或启发于网络资源（可能部分未详尽说明），工程基于Unity版本5.3.4f1。

## 简介

### 01.AssetsSettings 

资源导入配置

启发于[Unity实用小工具：Asset Auditing](http://forum.china.unity3d.com/forum.php?mod=viewthread&tid=19957&extra=page%3D1%26filter%3Dtypeid%26typeid%3D18)

配置好资源导入规则文件放到文件夹中，资源导入时，向上查找最近的一个符合条件的规则进行配置。

规则包括：

- 常规的模型和贴图的设置项
- 自定义的路径过滤

![图](http://odk2uwdl8.bkt.clouddn.com/2016-09-19-my-unity-common-tools_00.png)

TODO:
- [ ] 很大的一个弊端是各个类型要自己定义，并且后期可能不兼容
- [x] 如何暂时停止设置，进行调试，InUse为false，只是跳过这一个，会继续往上找
    - 增加了TmpIgnore
- [ ] 加上对bundle的设置
- [ ] 增加设置`DrawSelected`接口
- [ ] 对bundle的设置
    - 识别的类型
- [ ] 区分不同平台，比如，Android和IOS不一样的图片设置
- 支持的资源类型
    - [x] 模型
    - [ ] 贴图
    - [ ] RenderTexture
    - [ ] 音效

另一个类似的工具：[AssetGraph](https://github.com/laijingfeng/AssetGraph)

### 02.BakePrefab

Prefab烘焙

### 03.Collect

收集整理中的

### 04.Common

通用工具

- JerryMath 常用数学库
- Util 常用小功能
- DrawGizmos 绘制一个结点的位置信息，可用来快速显示大量结点

### 05.Drawer

#### Drawer

绘制辅助图工具，结构参考iTween

对外接口：
- `string Add(Hashtable table)` 增加，返回id
- `Hashtable Hash(params object[] args)` 构建参数
- `bool Exist(string id)`
- Remove
    - `void Remove(Hashtable table)`
    - `void Remove(string id)`
    - `void RemoveAll()`

参数：
- id id
- type 类型，DrawType
- from 起点
- to 终点
- pos 位置
- size 大小
- size_factor 大小比例 
- delay 延时秒数删除
- color 颜色
- wire 网格
- text 文本

#### Drawer2

Drawer的进阶版，接口更方便，面向对象

对外接口：
- `T Draw<T>() where T : Drawer2ElementBase`
- `T GetElement<T>(string id) where T : Drawer2ElementBase`
- `void Remove(Drawer2ElementBase ele)`
- `void RemoveByID(string id)`
- `void RemoveAll()`

其他接口都用`SetXXX`形式，如：
- SetColor
- SetDelayDelete
- SetWire
- SetSize
- SetSizeFactor

目前支持的类型：
- `Drawer2ElementPath` 路径、线段、多边形
- `Drawer2ElementCube` 立方体
- `Drawer2ElementLabel` 标签

Tip：
- 要在编辑器起作用，测试脚本要加上`[ExecuteInEditMode]`
    - 另外修改了代码会没有，重新开一些场景
- UGUI上绘制的只有Scene才能看到，Game看不到 

> 接口用`SetXXX`形式，不在构造函数里填基本参数，基于两点考虑：
>
> 一是加新的类型的时候只需要加类就好了，不要去注册接口
>
> 二是`SetXXX`形式的接口可以随时修改所有参数，不便之处是每个类都要把父对象的`SetXXX`接口再包装一下以便返回的类型是自己的类型

进阶：
- [ ] `SetXXX`是否可以做到子类不覆盖自动支持
- [ ] 支持挂载在结点编辑器使用
- [ ] Drawer可以删掉了？
- [ ] 增加多边形面的绘制
    - 画三角形
    - 颜色可以设置alpha吗？

### 06.ConditionalHideAttribute 

编辑器工具，按条件隐藏属性

示例见WayPointMgr

### 07.CurveTool

动画曲线工具

### 08.EasyFramerateCount

帧率信息

EasyFramerateCounter参考EasyFramerateCount(Alterego Games)

### 09.EnumSort

枚举值显示自定义排序

### 10.FindReferences

查找资源的引用

功能：
- `Jerry/FindReferences/Find Setting`设置查找路径
- `Assets/Find References`查找引用，点击Log定位到宿主
- 对宿主通过`Assets/Find Detail`进一步查找细节，点击Log定位到具体结点（拖到Hierarchy里可以完全展开）

支持的类型：从prefab和unity中查找
- cs
- mat
- prefab
- ttf

参考[Unity3D研究院之查找资源被哪里引用了](http://www.xuanyusong.com/archives/4207)

### 11.FixMask

把Animation的Mask.Transform都勾上

来自[Fix mask button in the animation's inspector](http://forum.unity3d.com/threads/fix-mask-button-in-the-animations-inspector.224017/)

### 12.GestureJudge

手势判断

### 13.iTween

iTween，官方的，借用

TODO：
- [ ] 将要弃用

### 14.LoopScroll

循环无限ScrollView

### 15.MiniMap

来自Unity中国官方论坛

### 16.RadarChart

雷达图、多维属性图

![图](http://odk2uwdl8.bkt.clouddn.com/2016-09-19-my-unity-common-tools_01.png)

TODO：
- [ ] 锯齿

### 17.WayPointMgr

路点编辑和按路点运动

参考UnityExampleProject和iTween

### 18.Gradient

来自[UGUI研究院之Text文本渐变（十一）](http://www.xuanyusong.com/archives/3471)

### 19.EventMgrSys

事件管理

### 20.Singleton

单例