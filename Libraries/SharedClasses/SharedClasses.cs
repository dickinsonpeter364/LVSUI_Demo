using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SALS
{

    public class RoleChangeListener
    {
        //public void setMenus(object sender, RoleEventArgs e)
        //{
        //    try
        //    {
        //        List<string> allowedOptions = e.Menus;
        //        //ToolStripItemCollection tsiItems;
        //        MenuStrip ms = (MenuStrip)sender;
        //        if (ms == null)
        //            return;
        //        //else
        //        //    tsiItems = ms.Items;
        //        if (allowedOptions.Count == 0)
        //            return;

        //        foreach (ToolStripMenuItem tsi in ms.Items)
        //        {
        //            ms.Invoke((System.Windows.Forms.MethodInvoker)delegate
        //            {
        //                tsi.Enabled = allowedOptions.Contains(tsi.Name);
        //                tsi.Visible = allowedOptions.Contains(tsi.Name);
        //            });
        //            foreach (object tsub in tsi.DropDownItems)
        //            {
        //                Type t = tsub.GetType();
        //                if (t == typeof(ToolStripMenuItem))
        //                {
        //                    ToolStripMenuItem ts = (ToolStripMenuItem)tsub;
        //                    if (ts.Name == "mnuFileLogin")
        //                        if (e.RoleType != "")
        //                            ms.Invoke((System.Windows.Forms.MethodInvoker)delegate { ts.Text = "Switch User"; });
        //                        else
        //                            ms.Invoke((System.Windows.Forms.MethodInvoker)delegate { ts.Text = "Login"; });
        //                    ms.Invoke((System.Windows.Forms.MethodInvoker)delegate {
        //                        ts.Enabled = allowedOptions.Contains(ts.Name);
        //                        ts.Visible = allowedOptions.Contains(ts.Name);
        //                    });
        //                    if (ts.HasDropDownItems)
        //                        foreach (object tsubsub in ts.DropDownItems)
        //                        {
        //                            Type t1 = tsubsub.GetType();
        //                            if (t1 == typeof(ToolStripMenuItem))
        //                            {
        //                                ToolStripMenuItem ts1 = (ToolStripMenuItem)tsubsub;
        //                                ms.Invoke((System.Windows.Forms.MethodInvoker)delegate {
        //                                    ts1.Enabled = allowedOptions.Contains(ts1.Name);
        //                                    ts1.Visible = allowedOptions.Contains(ts1.Name);
        //                                });
        //                            }
        //                        }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string err = ex.Message;
        //    }
        //}
    }

}
