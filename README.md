项目 | 内容
---|---
标题 | MyUnityCommonTools
目录 | Github/项目
标签 | Github
备注 | 无
更新 | 2017-08-26 11:48:17

[TOC]

# 说明

常用的一些Unity项目公用组件，部分参考或启发于网络资源（可能部分未详尽说明），工程基于Unity版本5.3.4f1。

# 简介

## AssetsSettings 

搬迁到`2017-08-23-assets-settings.md`

## BakePrefab

Prefab烘焙

## Collect

收集整理中的

## ConditionalHideAttribute 

编辑器工具，按条件隐藏属性

示例见WayPointMgr

## CurveTool

动画曲线工具

## EasyFramerateCount

帧率信息

EasyFramerateCounter参考EasyFramerateCount(Alterego Games)

## EnumSort

枚举值显示自定义排序

## FixMask

把Animation的Mask.Transform都勾上

来自[Fix mask button in the animation's inspector](http://forum.unity3d.com/threads/fix-mask-button-in-the-animations-inspector.224017/)

## GestureJudge

手势判断

## iTween

iTween，官方的，借用

TODO：
- [ ] 将要弃用

## LoopScroll

循环无限ScrollView

### 新的

编辑器里填充：
- Prefab，这样只能放一种
- 宽度和高度，取Prefab的大小，应该是浮点数
- Spacing

### TODO

- [ ] 数据可能填不满
    - 所以不一定中间对齐 
- [ ] 数据可能排序、删除
- [ ] 缓存策略
    - 缓存至少`View + Add * 2`，减少Clone
    - 缓存所有，减少刷新
- [ ] 标记Dirty才更新

## MiniMap

来自Unity中国官方论坛

## RadarChart

雷达图、多维属性图

![图](http://odk2uwdl8.bkt.clouddn.com/2016-09-19-my-unity-common-tools_01.png)

TODO：
- [ ] 锯齿
- [ ] 边框线放大了是有BUG的，交汇点并不融合

收集：
- [在Unity中使用UGUI修改Mesh绘制几何图形](http://www.cnblogs.com/jeason1997/p/5130413.html)
- [下载](http://download.csdn.net/download/tankerhunter/9439789) 保存到百度网盘了
- http://download.csdn.net/detail/nippyli/9758792

## WayPointMgr

路点编辑和按路点运动

参考UnityExampleProject和iTween

## Gradient

来自[UGUI研究院之Text文本渐变（十一）](http://www.xuanyusong.com/archives/3471)

## Singleton

注意SingletonMono里Awake里的实例化不能去掉，预先挂载好的单例脚本要走这个实例化。

这里带来的一个不方便是子类的`Awake`必须注意要`override`了。

TODO:
- [x] Awake改成protected

## FastSwitchPlatform

使用：Windows/Fast...

参考：[Unity 快速平台切换](http://www.jianshu.com/p/91ff42fe5603)