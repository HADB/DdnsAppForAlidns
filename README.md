# README

## 配置

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup>
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5.2" />
    </startup>
    <appSettings>
        <add key="regionId" value="cn-hangzhou" />
        <add key="accessKeyId" value="你的AccessKeyId" />
        <add key="accessKeySecret" value="你的AccessKeySecret" />
        <add key="domains" value="test1.yourdomain.com,test2.yourdomain.com" />
        <add key="ttl" value="60" />
    </appSettings>
</configuration>
```

如上，注意阿里云云解析暂不支持RAM子账号，需要用老的Access Key来访问，有点坑。



## 定期运行

暂时没有做到程序中，可以通过计划任务来定时跑一下即可。



## 项目结构

由于阿里云的.net SDK在nuget上并没有统一的官方包，所以我是下载的[Open API SDK for .Net developers](https://github.com/aliyun/aliyun-openapi-net-sdk) 在本地引用的，需要编译的同学可以下载下来，重新链接下，用到了[aliyun-net-sdk-core](https://github.com/aliyun/aliyun-openapi-net-sdk/tree/master/aliyun-net-sdk-core) 和 [aliyun-net-sdk-alidns](https://github.com/aliyun/aliyun-openapi-net-sdk/tree/master/aliyun-net-sdk-alidns) 这两个项目。