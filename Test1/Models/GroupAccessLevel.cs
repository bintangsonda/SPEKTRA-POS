namespace IDS.Web.UI.Models
{
     public class GroupAccessLevel
    {
        public int ReadAccess { get; set; }
        public int CreateAccess { get; set; }
        public int EditAccess { get; set; }
        public int DeleteAccess { get; set; }
        public int ApproveAccess { get; set; }

        public GroupAccessLevel()
        {
            ReadAccess = -1;
            CreateAccess = -1;
            EditAccess = -1;
            DeleteAccess = -1;
            ApproveAccess = -1;
        }

        public static int GetGroupAccessLevel(string userGroupCode, string controller)
        {
            if (string.IsNullOrEmpty(userGroupCode) || string.IsNullOrEmpty(controller))
            {
                return 0;
            }

            Dictionary<string, List<IDS.Maintenance.GroupAccess>> dicAccesses = IDS.Tool.InMemoryCache.GetInstance().GetOrSet(IDS.Tool.GlobalVariable.CACHE_USER_GROUP_ACCESS, userGroupCode, () => IDS.Maintenance.GroupAccess.GetGroupAccess(userGroupCode), IDS.Tool.GlobalVariable.CACHE_DURATION_USER_GROUP_ACCESS);

            if (dicAccesses != null)
            {
                List<IDS.Maintenance.GroupAccess> accesses = dicAccesses[userGroupCode] as List<IDS.Maintenance.GroupAccess>;

                if (accesses != null)
                {
                    IDS.Maintenance.GroupAccess groupAccess = accesses.FirstOrDefault(x => x.ControllerName != null && x.ControllerName.Equals(controller, StringComparison.CurrentCultureIgnoreCase));

                    if (groupAccess == null)
                        return 0;
                    else
                        return groupAccess.Access;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        
        public static GroupAccessLevel GetFormGroupAccess(string userGroupCode, string controller)
        {
            GroupAccessLevel level = new GroupAccessLevel();

            if (string.IsNullOrEmpty(userGroupCode) || string.IsNullOrEmpty(controller))
                return level;
            else
            {
                int access = GetGroupAccessLevel(userGroupCode, controller);

                switch (access)
                {
                    case 1: // Read Only
                        level.ReadAccess = 1;
                        level.CreateAccess = 0;
                        level.EditAccess = 0;
                        level.DeleteAccess = 0;
                        level.ApproveAccess = 0;
                        break;
                    case 2:
                        level.ReadAccess = 1;
                        level.CreateAccess = 1;
                        level.EditAccess = 0;
                        level.DeleteAccess = 0;
                        level.ApproveAccess = 0;
                        break;
                    case 3:
                        level.ReadAccess = 1;
                        level.CreateAccess = 0;
                        level.EditAccess = 1;
                        level.DeleteAccess = 0;
                        level.ApproveAccess = 0;
                        break;
                    case 4:
                        level.ReadAccess = 1;
                        level.CreateAccess = 0;
                        level.EditAccess = 0;
                        level.DeleteAccess = 1;
                        level.ApproveAccess = 0;
                        break;
                    case 5:
                        level.ReadAccess = 1;
                        level.CreateAccess = 1;
                        level.EditAccess = 1;
                        level.DeleteAccess = 0;
                        level.ApproveAccess = 0;
                        break;
                    case 6:
                        level.ReadAccess = 1;
                        level.CreateAccess = 1;
                        level.EditAccess = 0;
                        level.DeleteAccess = 1;
                        level.ApproveAccess = 0;
                        break;
                    case 7:
                        level.ReadAccess = 1;
                        level.CreateAccess = 0;
                        level.EditAccess = 1;
                        level.DeleteAccess = 1;
                        level.ApproveAccess = 0;
                        break;
                    case 8:
                        level.ReadAccess = 1;
                        level.CreateAccess = 1;
                        level.EditAccess = 1;
                        level.DeleteAccess = 1;
                        level.ApproveAccess = 0;
                        break;
                    case 9:
                        level.ReadAccess = 1;
                        level.CreateAccess = 1;
                        level.EditAccess = 1;
                        level.DeleteAccess = 1;
                        level.ApproveAccess = 1;
                        break;
                    default:
                        level.ReadAccess = -1;
                        level.CreateAccess = -1;
                        level.EditAccess = -1;
                        level.DeleteAccess = -1;
                        break;
                }
            }

            return level;
        }

        /// <summary>
        /// Group Access untuk Web Form
        /// </summary>
        /// <param name="userGroupCode"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static int GetGroupAccessLevelByUrl(string userGroupCode, string url)
        {
            if (string.IsNullOrEmpty(userGroupCode) || string.IsNullOrEmpty(url))
            {
                return 0;
            }

            Dictionary<string, List<IDS.Maintenance.GroupAccess>> dicAccesses = IDS.Tool.InMemoryCache.GetInstance().GetOrSet(IDS.Tool.GlobalVariable.CACHE_USER_GROUP_ACCESS, userGroupCode, () => IDS.Maintenance.GroupAccess.GetGroupAccess(userGroupCode), IDS.Tool.GlobalVariable.CACHE_DURATION_USER_GROUP_ACCESS);

            if (dicAccesses != null)
            {
                List<IDS.Maintenance.GroupAccess> accesses = dicAccesses[userGroupCode] as List<IDS.Maintenance.GroupAccess>;

                if (accesses != null)
                {
                    IDS.Maintenance.GroupAccess groupAccess = accesses.FirstOrDefault(x => x.Url.Equals(url, StringComparison.CurrentCultureIgnoreCase));

                    if (groupAccess == null)
                        return 0;
                    else
                        return groupAccess.Access;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public static int GetGroupAccessLevelByMenuCode(string userGroupCode, string menuCode)
        {
            if (string.IsNullOrEmpty(userGroupCode) || string.IsNullOrEmpty(menuCode))
            {
                return 0;
            }

            Dictionary<string, List<IDS.Maintenance.GroupAccess>> dicAccesses = IDS.Tool.InMemoryCache.GetInstance().GetOrSet(IDS.Tool.GlobalVariable.CACHE_USER_GROUP_ACCESS, userGroupCode, () => IDS.Maintenance.GroupAccess.GetGroupAccess(userGroupCode), IDS.Tool.GlobalVariable.CACHE_DURATION_USER_GROUP_ACCESS);

            if (dicAccesses != null)
            {
                List<IDS.Maintenance.GroupAccess> accesses = dicAccesses[userGroupCode] as List<IDS.Maintenance.GroupAccess>;

                if (accesses != null)
                {
                    IDS.Maintenance.GroupAccess groupAccess = accesses.FirstOrDefault(x => x.MenuCode.Equals(menuCode, StringComparison.CurrentCultureIgnoreCase));

                    if (groupAccess == null)
                        return 0;
                    else
                        return groupAccess.Access;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
