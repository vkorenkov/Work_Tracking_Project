using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkTrackingLib.Models;

namespace WorkTracking_Server.Extentions
{
    public static class ServerExtentions
    {
        public static AccessModel ChangeVisibility(this AccessModel access)
        {
            if (access.Access < 1)
            {
                foreach (var a in access.GetType().GetProperties())
                {
                    if (a.Name.Contains("Visibility"))
                    {
                        a.SetValue(access, false);
                    }
                }

                return access;
            }
            else if (access.Access == 1)
            {
                foreach (var a in access.GetType().GetProperties())
                {
                    if (a.Name.Contains("Visibility") && a.Name != "VisibilityControlSql")
                    {
                        a.SetValue(access, true);
                    }
                }

                access.VisibilityControlSql = false;

                return access;
            }
            else
            {
                foreach (var a in access.GetType().GetProperties())
                {
                    if (a.Name.Contains("Visibility"))
                    {
                        a.SetValue(access, true);
                    }
                }

                return access;
            }
        }
    }
}
