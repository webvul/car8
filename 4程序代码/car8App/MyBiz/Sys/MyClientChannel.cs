using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCmn;
using System.ServiceModel;
using System.Diagnostics;
using System.ServiceModel.Channels;

namespace MyBiz
{
    public static class MyClientChannel
    {
        /// <summary>
        /// 从配置文件中取 T + Service 的配置节
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Tret Get<T, Tret>(Func<T, Tret> func)
        {
            //ChannelFactory<T> channel = CacheHelper.Get<ChannelFactory<T>>("MyClientChannel_" + typeof(T).FullName, CacheTime.None, () =>
            //    {

            //        return channelFactory;
            //    });
            //要缓存, 万次 6 秒
            using (ChannelFactory<T> channel = new ChannelFactory<T>(typeof(T).Name))
            {
                if (channel.State != CommunicationState.Opened)
                {
                    channel.Open();
                }


                var ch = channel.CreateChannel();



                var ret = func((T)ch);
                return ret;

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>http://www.cnblogs.com/dudu/archive/2011/11/02/wcf_client_no_using_call.html#wcfclient2</remarks>
        /// <typeparam name="TChannel"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TReturn Using<T, TReturn>(this T client, Func<T,TReturn> work)
            where T : ICommunicationObject
        {
            TReturn ret = default(TReturn);
            try
            {
                ret = work(client);
                client.Close();
            }
            catch (CommunicationException e)
            {
                client.Abort();
            }
            catch (TimeoutException e)
            {
                client.Abort();
            }
            catch (Exception e)
            {
                client.Abort();
                throw;
            }
            return ret;
        }
    }
}
