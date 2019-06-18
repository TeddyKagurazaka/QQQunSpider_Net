# QQQunSpider_Net
使用.Net实现的QQ群信息爬虫

## 怎么做的
手机版移动QQ调用HTTPS API获取群信息，这个API只需要腾讯统一认证（网页登陆）即可使用。  
所以要做的事情就是：模拟腾讯登录(使用浏览器)->请求API  

## 怎么用
打开这个项目的时候会初始化一个Chrome 75的窗口到腾讯的开发者平台并要求登录  
*由于是调用腾讯官网登陆并获取Cookie，程序本身**不收集**登录情报(用户名和密码)*  
完成登录后Chrome会自动关闭，程序通过Selenium得到UIN和Key，用此请求API得到群基本数据

## 怎么批量用
保存一个Input.txt文件在本地，每行一个关键字  
完成了验证步骤之后，程序会自动读取关键字获取群信息，并保存原始JSON（关键字.txt）

## 能获取的内容
+ 群ID
+ 群主ID
+ 群名
+ 群简介
+ 群标签（群人数、分类、管理在线等）
+ 群头像

## 程序有限制
+ 由于鹅厂侧的限制，若结果大于500条则只会返回前500条数据，后面的数据鹅厂会标志isEnd为true导致无法获取
+ 一个QQ号API请求大约每小时100次左右，**超出请求会报anti-malicious拉黑，同时该账号正常群请求（手机客户端搜群）也会被封禁**
+ 以下部分走的腾讯自有QQ协议，无法获取
  + 群管理信息
