using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.Compilation;

public class PackageChecker
{
    private static AddRequest addRequest;
    private static ListRequest listRequest;
    private static Stack<int> missingPackages = new Stack<int>();

    public class PackageEntry
    {
        public string name;
        public string version;
    }

    private static List<PackageEntry> packageToAdd;

    [InitializeOnLoadMethod]
    static void CheckPackage()
    { 
        string filePath = Application.dataPath + "/../Library/PackageChecked";

        if (!File.Exists(filePath))
        {
            Debug.Log("[Auto Package] : Checking if required packages are included");

            var packageListFile = Directory.GetFiles(Application.dataPath, "PackageImportList.txt", SearchOption.AllDirectories);
            if (packageListFile.Length == 0)
            {
                Debug.LogError("[Auto Package] : Couldn't find the packages list. Be sure there is a file called PackageImportList in your project");
            }
            else
            {
                string packageListPath = packageListFile[0];
                packageToAdd = new List<PackageEntry>();
                string[] content = File.ReadAllLines(packageListPath);
                foreach (var line in content)
                {
                    var split = line.Split('@');
                    PackageEntry entry = new PackageEntry();

                    entry.name = split[0];
                    entry.version = split.Length > 1 ? split[1] : null;

                    packageToAdd.Add(entry); 
                }

                File.WriteAllText(filePath, "Delete this to trigger a new auto package check");
                listRequest = Client.List();
                EditorApplication.update += Update;
            }
        }
    }

    static void Update()
    {
        if (listRequest != null)
        {
            if (listRequest.IsCompleted)
            {
                bool[] foundPackages = new bool[packageToAdd.Count];

                for (int i = 0; i < foundPackages.Length; ++i)
                    foundPackages[i] = false;

                foreach (var package in listRequest.Result)
                {
                    for (int i = 0; i < foundPackages.Length; ++i)
                    {
                        if (package.packageId.Contains(packageToAdd[i].name))
                        {
                            foundPackages[i] = true;
                            Debug.Log("[Auto package] Package " + packageToAdd[i].name + " already imported in that project");
                        }
                    }
                }

                for (int i = 0; i < foundPackages.Length; ++i)
                {
                    if (!foundPackages[i])
                        missingPackages.Push(i);
                }

                listRequest = null;
            }
            else if (listRequest.Error != null)
            {
                Debug.Log(listRequest.Error.message);
                listRequest = null;
            }
        }
        else
        {
            bool noMorePackage = false;

            if(missingPackages.Count > 0)
                EditorUtility.DisplayProgressBar("Importing package", "Importing missing package for the project", 1.0f - missingPackages.Count/(float)packageToAdd.Count);
            else
                EditorUtility.ClearProgressBar();

            if (addRequest == null)
            {
                if (missingPackages.Count == 0)
                {
                    noMorePackage = true;
                }
                else
                {
                    int package = missingPackages.Pop();
                    string name = packageToAdd[package].name;
                    if (packageToAdd[package].version != null)
                        name += "@" + packageToAdd[package].version;

                    addRequest = Client.Add(name);
                }
            }
            else
            {
                if (addRequest.IsCompleted)
                {
                    if (addRequest.Error != null)
                    {
                        Debug.LogError("[Auto Package Error] : " + addRequest.Error.message);
                    }
                    else if (addRequest.Result != null)
                    {
                        Debug.Log("[Auto Package] : Automatically added package " + addRequest.Result.displayName);
                    }
                    else
                    {
                        Debug.LogError("[Auto Package] : Unknown error with adding new package to the Package Manager");
                    }

                    addRequest = null;
                }
            }

            if (noMorePackage)
            {
                Debug.Log("[Auto Package] : All packages checked");
                EditorApplication.update -= Update;
            }
        }
    }
}

