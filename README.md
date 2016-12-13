#### Contents
- [MyUnityCommonTools](#myunitycommontools)
    - [说明](#)
    - [简介](#)
        - [01.AssetsSettings](#01assetssettings)
        - [02.BakePrefab](#02bakeprefab)
        - [03.Collect](#03collect)
        - [04.Common](#04common)
        - [05.ConditionalHideAttribute](#05conditionalhideattribute)
        - [06.CurveTool](#06curvetool)
        - [07.EasyFramerateCount](#07easyframeratecount)
        - [08.EnumSort](#08enumsort)
        - [09.FindReferences](#09findreferences)
        - [10.FixMask](#10fixmask)
        - [11.Fsm](#11fsm)
        - [12.GestureJudge](#12gesturejudge)
        - [13.iTween](#13itween)
        - [14.LoopScroll](#14loopscroll)
        - [15.MiniMap](#15minimap)
        - [16.RadarChart](#16radarchart)
        - [17.WayPointMgr](#17waypointmgr)
        - [18.Gradient](#18gradient)
        - [19.EventMgrSys](#19eventmgrsys)

项目 | 内容
---|---
标题 | MyUnityCommonTools
目录 | Github
标签 | Github
备注 | 无
最近更新 | 2016-12-14 00:59:13

# MyUnityCommonTools

## 说明

常用的一些Unity项目公用组件，部分参考或启发于网络资源（可能部分未详尽说明），工程基于Unity版本5.3.4f1。

## 简介

### 01.AssetsSettings 

资源导入配置，配置项细节继续完善中...

启发于[Unity实用小工具：Asset Auditing](http://forum.china.unity3d.com/forum.php?mod=viewthread&tid=19957&extra=page%3D1%26filter%3Dtypeid%26typeid%3D18)

配置好资源导入规则文件放到文件夹中，资源导入时，向上查找最近的一个符合条件的规则进行配置。

规则包括：

- 常规的模型和贴图的设置项
- 自定义的路径过滤

![图](http://odk2uwdl8.bkt.clouddn.com/2016-09-19-my-unity-common-tools_00.png)

### 02.BakePrefab

Prefab烘焙

### 03.Collect

收集整理中的

### 04.Common

通用工具

- JerryMath 常用数学库
- Util 常用小功能
- Drawer 绘制辅助图工具，结构参考iTween
- DrawGizmos 绘制一个结点的位置信息，可用来快速显示大量结点

### 05.ConditionalHideAttribute 

编辑器工具，按条件隐藏属性

示例见WayPointMgr

### 06.CurveTool

动画曲线工具

### 07.EasyFramerateCount

帧率信息

EasyFramerateCounter参考EasyFramerateCount(Alterego Games)

### 08.EnumSort

枚举值显示自定义排序

### 09.FindReferences

查找资源的引用

参考[Unity3D研究院之查找资源被哪里引用了](http://www.xuanyusong.com/archives/4207)

### 10.FixMask

把Animation的Mask.Transform都勾上

来自[Fix mask button in the animation's inspector](http://forum.unity3d.com/threads/fix-mask-button-in-the-animations-inspector.224017/)

### 11.Fsm

一个简单的状态机

### 12.GestureJudge

手势判断

### 13.iTween

iTween，官方的，借用

### 14.LoopScroll

循环无限ScrollView

### 15.MiniMap

来自Unity中国官方论坛

### 16.RadarChart

雷达图、多维属性图

![图](http://odk2uwdl8.bkt.clouddn.com/2016-09-19-my-unity-common-tools_01.png)

### 17.WayPointMgr

路点编辑和按路点运动

参考UnityExampleProject和iTween

### 18.Gradient

来自[UGUI研究院之Text文本渐变（十一）](http://www.xuanyusong.com/archives/3471)

### 19.EventMgrSys

事件管理