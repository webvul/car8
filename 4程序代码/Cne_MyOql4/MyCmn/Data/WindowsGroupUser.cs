using System;
using System.DirectoryServices;  //这两个using一定要写上去
using System.DirectoryServices.Protocols;
using System.Collections;
using System.Collections.Generic;//要在reference里添加System.DirectoryServices.dll和System.DirectoryServices.Protocols.dll

namespace MyCmn
{
    /// <summary>
    /// 主要包括下面两个方法，其他方法也可以用，主要为辅助主方法用
    /// UpdatePassWord(string var)  修改本地密码的方法
    /// GetUserInfoArrayList(string var)    根据用户组,查询本地包含用户HashTable(含名称、全名、描述）的数组
    /// </summary>
    public class WindowsGroupUser
    {
        public class UserInfo
        {
            public string UserName { get; set; }
            public string FullName { get; set; }
            public string Description { get; set; }
        }
        /// <summary>
        /// 修改本地密码的方法
        /// </summary>
        /// <param name="intputPwd">输入的新密码</param>
        /// <returns>成功返回"success",失败返回exception</returns>
        public static string UpdatePassWord(string intputPwd)
        {
            try
            {
                DirectoryEntry AD = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");
                DirectoryEntry user = AD.Children.Find("test", "User");
                user.Invoke("SetPassword", new object[] { intputPwd });
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        /// <summary>
        /// 根据本地用户组获得组里的用户名数组
        /// </summary>
        /// <param name="directoryEntry">=new DirectoryEntry("WinNT://" + Environment.MachineName + "/" + localGroup + ",group")</param>
        /// <returns>用户名数组</returns>
        public static List<string> GetUsersArrayList(DirectoryEntry directoryEntry)
        {
            var arrUsers = new List<string>();
            try
            {
                foreach (object member in (IEnumerable)directoryEntry.Invoke("Members"))
                {
                    DirectoryEntry dirmem = new DirectoryEntry(member);
                    arrUsers.Add(dirmem.Name);
                }
                return arrUsers;
            }
            catch { return arrUsers; }

        }
        /// <summary>
        /// 获得每个单独的用户信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="directoryEntry">目录入口</param>
        /// <returns>单独用户信息的HashTable</returns>
        public static UserInfo GetSingleUserInfo(string userName, string localGroup)
        {

            try
            {
                DirectoryEntry group = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");
                try
                {
                    System.DirectoryServices.DirectoryEntry user = group.Children.Find(userName, "User");
                    string FullName = Convert.ToString(user.Invoke("Get", new object[] { "FullName" }));
                    string Description = Convert.ToString(user.Invoke("Get", new object[] { "Description" }));

                    return new UserInfo() { UserName = userName, FullName = FullName, Description = Description };
                }
                catch { };

            }
            catch { }
            return null;
        }
        /// <summary>
        /// 根据用户组,查询本地包含用户HashTable(含名称、全名、描述）的数组
        /// </summary>
        /// <param name="localGroup">用户组名称</param>
        /// <returns>包含用户HashTable(含名称、全名、描述）的数组</returns>
        public static List<UserInfo> GetUserInfoArrayList(string localGroup)
        {
            var arr = new List<UserInfo>();//al返回HASHTABLE数组用

            try
            {
                DirectoryEntry group = new DirectoryEntry("WinNT://" + Environment.MachineName + "/" + localGroup + ",group");

                foreach (string user in GetUsersArrayList(group))
                {
                    arr.Add(GetSingleUserInfo(user, localGroup));
                }
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
            }
            return arr;
        }
    }
}