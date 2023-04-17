using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Vim.G3d.Tests;

public static class TestUtils
{
    public static readonly string ProjectFolder = new DirectoryInfo(Properties.Resources.ProjDir.Trim()).FullName;
    public static string RootFolder = Path.Combine(ProjectFolder, "..", "..");
    public static string TestInputFolder = Path.Combine(RootFolder, "data", "models");
    public static string TestOutputFolder = Path.Combine(RootFolder, "out");

    public static string PrepareTestDir([CallerMemberName] string? testName = null)
    {
        if (testName == null)
            throw new ArgumentException(nameof(testName));

        var testDir = Path.Combine(TestOutputFolder, testName);

        // Prepare the test directory
        if (Directory.Exists(testDir))
            Directory.Delete(testDir, true);
        Directory.CreateDirectory(testDir);

        return testDir;
    }

    public static (long, long) GetMemoryConsumptionAndMSecElapsed(Action action)
    {
        var time = 0L;
        var mem = GetMemoryConsumption(
            () => time = GetMSecElapsed(action));
        return (mem, time);
    }

    public static long GetMSecElapsed(Action action)
    {
        var sw = Stopwatch.StartNew();
        action();
        return sw.ElapsedMilliseconds;
    }

    /// <summary>
    /// Creates a directory if needed, or clears all of its contents otherwise
    /// </summary>
    public static string CreateAndClearDirectory(string dirPath)
    {
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
        else
            DeleteFolderContents(dirPath);
        return dirPath;
    }

    /// <summary>
    /// Deletes all contents in a folder
    /// </summary>
    /// <remarks>
    /// https://stackoverflow.com/questions/1288718/how-to-delete-all-files-and-folders-in-a-directory
    /// </remarks>
    private static void DeleteFolderContents(string folderPath)
    {
        var di = new DirectoryInfo(folderPath);
        foreach (var dir in di.EnumerateDirectories().AsParallel())
            DeleteFolderAndAllContents(dir.FullName);
        foreach (var file in di.EnumerateFiles().AsParallel())
            file.Delete();
    }

    /// <summary>
    /// Deletes everything in a folder and then the folder.
    /// </summary>
    private static void DeleteFolderAndAllContents(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            return;

        DeleteFolderContents(folderPath);
        Directory.Delete(folderPath);
    }

    // NOTE: Calling a function generates additional memory
    private static long GetMemoryConsumption(Action action)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        var memBefore = GC.GetTotalMemory(true);
        action();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        return GC.GetTotalMemory(true) - memBefore;
    }
}