using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class PackageExporterTests
{
    public class AssetPathRetrieverTest
    {
        [Test]
        public void ProjectRootDirTests()
        {
            string validProjectDir = "D:/devenv/Unity projects/Unity Packages Development/";

            string[] invalidSeparatorDirs = new string[]
            {
                "D:\\devenv\\Unity projects\\Unity Packages Development\\",
                "D:\\devenv\\Unity projects/Unity Packages Development\\",
            };

            string[] invalidDirs = new string[]
            {
                "D:/devenv/Unity projects/Unity Packages Development/Assets/",
                "D:/devenv/Unity projects/Unity Packages Development/Assets",
            };

            string testProjectDir = AssetFinder.ProjectRootDir;

            Assert.AreEqual(validProjectDir, testProjectDir);

            foreach (string dir in invalidSeparatorDirs)
            {
                Assert.AreEqual(testProjectDir, dir.UnifyDirPath());
            }

            foreach (string dir in invalidDirs)
            {
                Assert.AreNotEqual(dir, testProjectDir);
            }
        }

        [InitializeOnLoadMethod]
        public static void TestOnLoad()
        {
            //Type t = typeof(DummyScript);
            //Debug.Log(AssetPathRetriever.GetAssetsPath($"t:DummyScript").FirstOrDefault());
        }
    }
}
