using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

public enum ScriptStatus
{
    Actual,
    Compiling,
    Compiled,
    HasChanges
}

public class ScriptStatusMonitor : IDisposable
{
    public event Action<ScriptStatus> DomainStatusChanged;
    public ScriptStatus Status { get; private set; } = ScriptStatus.Actual;

    private readonly ScriptChangeMonitor _scriptChangeMonitor;

    public ScriptStatusMonitor(ScriptChangeMonitor scriptChangeMonitor)
    {
        _scriptChangeMonitor = scriptChangeMonitor;
    }

    private bool _enabled = false;
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (value == _enabled)
                return;

            if (value)
            {
                CompilationPipeline.compilationStarted += OnCompilationStarted;
                CompilationPipeline.compilationFinished += OnCompilationFinished;

                AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
                AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;

                EnableScriptTracking();

                _enabled = true;
            }
            else
            {
                CompilationPipeline.compilationStarted -= OnCompilationStarted;
                CompilationPipeline.compilationFinished -= OnCompilationFinished;

                AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
                AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;

                DisableScriptTracking();

                _enabled = false;
            }
        }
    }

    private void OnBeforeAssemblyReload()
    {

    }

    private void OnAfterAssemblyReload()
    {

    }

    public bool HasScriptChanges { get; private set; } = false;
    public bool IsCompiling { get; private set; } = false;

    private void OnCompilationStarted(object context)
    {
        IsCompiling = true;

        DomainStatusChanged?.Invoke(ScriptStatus.Compiling);
        Status = ScriptStatus.Compiling;

        HasScriptChanges = true;
    }

    private void OnCompilationFinished(object context)
    {
        IsCompiling = false;

        DomainStatusChanged?.Invoke(ScriptStatus.Compiled);
        Status = ScriptStatus.Compiled;

        _scriptChangeMonitor.Reset();
        HasScriptChanges = false;
    }

    private void EnableScriptTracking()
    {
        _scriptChangeMonitor.ScriptChanged += OnScriptChanged;
        _scriptChangeMonitor.Enabled = true;
    }

    private void DisableScriptTracking()
    {
        _scriptChangeMonitor.ScriptChanged -= OnScriptChanged;
        _scriptChangeMonitor.Enabled = false;
    }

    private void OnScriptChanged(ScriptChangedEventArgs e)
    {
        DomainStatusChanged?.Invoke(ScriptStatus.HasChanges);
        Status = ScriptStatus.HasChanges;

        HasScriptChanges = true;
    }

    public void Dispose()
    {
        Enabled = false;

        if (_scriptChangeMonitor is IDisposable disposable)
            disposable.Dispose();
    }
}

public class FileSystemWatcherScriptChangeTracker : IScriptChangeTracker, IDisposable
{
    private FileSystemWatcher _watcher;
    private bool _enabled = false;

    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            if (value == _enabled)
                return;

            if (value == true)
            {
                _watcher.Changed += OnScriptChanged;
                _watcher.Created += OnScriptChanged;
                _watcher.Deleted += OnScriptChanged;
                _watcher.Renamed += OnScriptChanged;
                _watcher.EnableRaisingEvents = true;
            }
            else
            {
                _watcher.Changed -= OnScriptChanged;
                _watcher.Created -= OnScriptChanged;
                _watcher.Deleted -= OnScriptChanged;
                _watcher.Renamed -= OnScriptChanged;
                _watcher.EnableRaisingEvents = false;
            }

            _enabled = value;
        }
    }

    public FileSystemWatcherScriptChangeTracker()
    {
        _watcher = new FileSystemWatcher(Application.dataPath, "*.cs")
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
        };
    }

    public event Action<ScriptChangedEventArgs> ScriptChanged;

    private void OnScriptChanged(object sender, FileSystemEventArgs e)
    {
        ScriptChanged?.Invoke(new ScriptChangedEventArgs(e.Name, e.FullPath));
    }

    public void Dispose()
    {
        Enabled = false;
        _watcher.Dispose();
    }
}

public class ScriptChangedEventArgs
{
    public string name;
    public string fullPath;

    public ScriptChangedEventArgs(string name, string fullPath)
    {
        this.name = name;
        this.fullPath = fullPath;
    }
}

public interface IScriptChangeTracker
{
    bool Enabled { get; set; }

    event Action<ScriptChangedEventArgs> ScriptChanged;
}

public class ScriptChangeMonitor : IDisposable
{
    private bool _enabled = false;
    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            if (value == _enabled)
                return;

            if (value == true)
            {
                _changeTracker.ScriptChanged += OnScriptChanged;
                _changeTracker.Enabled = true;
            }
            else
            {
                _changeTracker.ScriptChanged -= OnScriptChanged;
                _changeTracker.Enabled = false;

                _changedScripts.Clear();
            }

            _enabled = value;
        }
    }

    private readonly IScriptChangeTracker _changeTracker;

    private HashSet<string> _changedScripts = new();
    public void Reset() => _changedScripts.Clear();

    public event Action<ScriptChangedEventArgs> ScriptChanged;

    public ScriptChangeMonitor(IScriptChangeTracker scriptChangeTracker)
    {
        _changeTracker = scriptChangeTracker;
    }

    private void OnScriptChanged(ScriptChangedEventArgs e)
    {
        _changedScripts.Add(e.fullPath);

        ScriptChanged?.Invoke(e);
    }

    public void Dispose()
    {
        Enabled = false;

        if (_changeTracker is IDisposable disposable)
            disposable.Dispose();
    }
}