# ET7Server
 基于ET7.2制作的ET独立Server版本,目的是适配各种非ET客户端.    
 数学库换成了System.Numeric  
 Excel目录是配置,导表只导出Json.正式使用自行替换一套导表  
 Proto目录是协议,项目里基于proto-net的,如果你的客户端不是C#.请自行搭配一套适合前后端的.常见是使用protobuf.  

 启动时可能报Http服务器的权限错误.确定自己的账号是系统管理员身份,然后cmd运行下面这句话就好了:  
 netsh http add urlacl url=http:/*:30300/ user=Everyone   
 其他的没什么,打开Sln编译一下,直接跑就行了.
 有问题欢迎反馈.
