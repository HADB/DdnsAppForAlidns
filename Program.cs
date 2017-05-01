using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using Aliyun.Acs.Alidns.Model.V20150109;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Newtonsoft.Json;
using NLog;
using static Aliyun.Acs.Alidns.Model.V20150109.DescribeSubDomainRecordsResponse;

namespace DdnsAppForAlidns
{
    class Program
    {
        static string regionId = ConfigurationManager.AppSettings["regionId"];
        static string accessKeyId = ConfigurationManager.AppSettings["accessKeyId"];
        static string accessKeySecret = ConfigurationManager.AppSettings["accessKeySecret"];
        static long ttl = long.Parse(ConfigurationManager.AppSettings["ttl"]);
        static string[] domains = ConfigurationManager.AppSettings["domains"].Split(',');
        static string currentDomain = "";
        static string currentIp = "";
        static DefaultProfile profile = DefaultProfile.GetProfile(regionId, accessKeyId, accessKeySecret);
        static DefaultAcsClient Client = new DefaultAcsClient(profile);
        static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            currentIp = GetIp();
            if (currentIp == null)
            {
                Log(string.Format("获取外网IP地址失败"));
            }
            else
            {
                foreach (var domain in domains)
                {
                    currentDomain = domain;
                    var request = new DescribeSubDomainRecordsRequest();
                    request.SubDomain = domain.Trim();
                    var response = Client.GetAcsResponse(request);
                    var records = response.DomainRecords;
                    if (records.Count == 0)
                    {
                        Log(string.Format("{0} 暂无解析", currentDomain));
                        AddRecord();
                    }
                    else
                    {
                        var record = records[0];
                        Log(string.Format("{0} 解析值为 {1}", currentDomain, record.Value));


                        if (currentIp == record.Value)
                        {
                            Log(string.Format("{0} 无须更新", currentDomain));
                        }
                        else
                        {
                            UpdateRecord(record);
                        }
                    }
                }
            }
        }

        static string GetIp()
        {
            try
            {
                using (var response = WebRequest.Create("http://api.monkeyrun.net/api/public/ip").GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        var result = JsonConvert.DeserializeObject<GetIpResult>(reader.ReadToEnd());

                        if (result.Success)
                        {
                            return result.IpAddress;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            return null;
        }

        static void AddRecord()
        {
            var domainParts = currentDomain.Split('.');
            var rr = "@";
            var domainName = currentDomain;
            if (domainParts.Length >= 3)
            {
                var rrParts = new List<string>();
                for (var i = 0; i < domainParts.Length - 2; i++)
                {
                    rrParts.Add(domainParts[i]);
                }
                rr = string.Join(".", rrParts);

                var domainNameParts = new List<String>();
                for (var i = domainParts.Length - 2; i < domainParts.Length; i++)
                {
                    domainNameParts.Add(domainParts[i]);
                }
                domainName = string.Join(".", domainNameParts);
            }
            var request = new AddDomainRecordRequest
            {
                RR = rr,
                DomainName = domainName,
                Type = "A",
                Value = currentIp,
                TTL = ttl
            };
            var response = Client.GetAcsResponse(request);
            if (response.HttpResponse.isSuccess())
            {
                Log(string.Format("{0} 解析已添加 {1}", currentDomain, currentIp));
            }
            else
            {
                Log(string.Format("{0} 解析添加失败 {1}", currentDomain, response.HttpResponse.Content));
            }
        }

        static void UpdateRecord(Record record)
        {
            var request = new UpdateDomainRecordRequest
            {
                RecordId = record.RecordId,
                RR = record.RR,
                Type = record.Type,
                Value = currentIp,
                TTL = record.TTL
            };

            var response = Client.GetAcsResponse(request);
            if (response.HttpResponse.isSuccess())
            {
                Log(string.Format("{0} 的解析已更新为 {1}", currentDomain, currentIp));
            }
            else
            {
                Log(string.Format("{0} 的解析更新失败 {1}", currentDomain, response.HttpResponse.Content));
            }
        }

        static void Log(string text)
        {
            logger.Info(text);
            Console.WriteLine(text);
        }
    }
}
