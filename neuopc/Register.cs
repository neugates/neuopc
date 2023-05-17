using System;
using System.Collections.Generic;
using System.Text;
using OPCAutomation;

namespace neuopc
{
    class Register
    {
        public static void Setup()
        {
            int count = 3;
            do
            {
                bool flag = true;
                try
                {
                    var testServer = new OPCServer();
                }
                catch (Exception)
                {
                    flag = false;
                }

                if (flag)
                {
                    break;
                }

                // TODO: regist com component
                count--;
            } while (0 < count);
        }
    }
}
