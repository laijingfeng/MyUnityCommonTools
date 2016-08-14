# MyUnityCommonTools

## 说明

常用的一些Unity项目公用组件，部分参考或启发于网络资源（可能部分未详尽说明），工程基于Unity版本5.3.4f1。

## 简介

- Fsm 一个简单的状态机
- RadarChart 雷达图、多维属性图
- WayPointMgr 路点编辑和按路点运动 
	- 参考UnityExampleProject和iTween
- JerryDebug 在移动端打印LOG，支持文件和结构，配置一些指令入口
- EnumSort 枚举值显示自定义排序
- JerryMath 常用数学库
- Util 常用小功能
- ConditionalHideAttribute 编辑器工具，按条件隐藏属性
	- 示例见WayPointMgr
- Drawer 绘制辅助图工具，结构参考iTween
- DrawGizmos 绘制一个结点的位置信息，可用来快速显示大量结点
- GestureJudge 手势判断
- FPS FPS和内存显示，完善中...

### AssetsSettings 

资源导入配置，配置项细节继续完善中...

启发于[Unity实用小工具：Asset Auditing](http://forum.china.unity3d.com/forum.php?mod=viewthread&tid=19957&extra=page%3D1%26filter%3Dtypeid%26typeid%3D18)

配置好资源导入规则文件放到文件夹中，资源导入时，向上查找最近的一个符合条件的规则进行配置。

规则包括：

- 常规的模型和贴图的设置项
- 自定义的路径过滤

![图1](http://laijingfeng.github.io/MyUnityCommonTools/images/image00.png)

### FixMask

把Animation的Mask.Transform都勾上

来自[Fix mask button in the animation's inspector](http://forum.unity3d.com/threads/fix-mask-button-in-the-animations-inspector.224017/)