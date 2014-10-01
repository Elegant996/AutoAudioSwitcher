using AutoAudioSwitcher.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace AutoAudioSwitcher
{
    public class Program
    {
        static void Main()
        {
            var swapped = false;
            var deviceList = GetDevices();
            using (var iter = deviceList.GetEnumerator())
            {
                if (iter.MoveNext())
                {
                    var tuple = iter.Current;
                    var id = tuple.Item1;
                    var deviceName = tuple.Item2;
                    var isInUse = tuple.Item3;
                    while (iter.MoveNext())
                    {
                        id = tuple.Item1;
                        deviceName = tuple.Item2;
                        isInUse = tuple.Item3;
                        if (!swapped && isInUse)
                            SelectDevice(id+1);

                        tuple = iter.Current;
                    }

                    id = tuple.Item1;
                    deviceName = tuple.Item2;
                    isInUse = tuple.Item3;
                    if (!swapped && isInUse)
                        SelectDevice(1);
                }
            }
        }

        #region EndPointController.exe interaction

        private static IEnumerable<Tuple<int, string, bool>> GetDevices()
        {
            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = "EndPointController.exe",
                    Arguments = "-f \"%d|%ws|%d|%d\""
                }
            };
            p.Start();
            p.WaitForExit();
            var stdout = p.StandardOutput.ReadToEnd().Trim();

            var devices = new List<Tuple<int, string, bool>>();

            foreach (var line in stdout.Split('\n'))
            {
                var elems = line.Trim().Split('|');
                var deviceInfo = new Tuple<int, string, bool>(int.Parse(elems[0]), elems[1], elems[3].Equals("1"));
                devices.Add(deviceInfo);
            }

            return devices;
        }

        private static void SelectDevice(int id)
        {
            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = "EndPointController.exe",
                    Arguments = id.ToString(CultureInfo.InvariantCulture)
                }
            };
            p.Start();
            p.WaitForExit();
        }

        #endregion
    }
}