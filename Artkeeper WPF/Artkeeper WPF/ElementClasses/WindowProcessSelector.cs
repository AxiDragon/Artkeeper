using Artkeeper.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

namespace Artkeeper.ElementClasses
{
    internal class WindowProcessSelector
    {
        //this is a list of processes that have windows but are not visible/useful to the user to select
        private static readonly List<string> excludedWindows = new List<string>{ "SystemSettings.exe", "ApplicationFrameHost.exe", "TextInputHost.exe" };
        
        private ComboBox windowSelector;
        private Process selectedProcess;
        private List<Process> windowProcesses = new List<Process>();

        public WindowProcessSelector(ComboBox windowSelector)
        {
            this.windowSelector = windowSelector;
            UpdateDropDownMenu();

            windowSelector.DropDownOpened += OnDropDownOpened;
            windowSelector.SelectionChanged += OnSelectionChanged;
            windowSelector.SelectedIndex = 0;

            selectedProcess = windowProcesses[0];
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (windowSelector.SelectedIndex >= 0)
            {
                selectedProcess = windowProcesses[windowSelector.SelectedIndex];
            }
        }

        private void OnDropDownOpened(object? sender, EventArgs e)
        {
            UpdateDropDownMenu();
        }

        private void UpdateDropDownMenu()
        {
            int currentSelectionId = -1;

            if (selectedProcess != null)
            {
                currentSelectionId = selectedProcess.Id;
            }

            windowSelector.Items.Clear();

            windowProcesses = GetWindowProcesses();

            int selectionIndex = 0;

            for (int i = 0; i < windowProcesses.Count; i++)
            {
                Process process = windowProcesses[i];

                windowSelector.Items.Add(process.GetProcessInfoString(true));

                if (process.Id == currentSelectionId)
                {
                    selectionIndex = i;
                }
            }

            windowSelector.SelectedIndex = selectionIndex;
        }

        private List<Process> GetWindowProcesses()
        {
            List<Process> windowProcesses = new List<Process>();

            Process[] processList = Process.GetProcesses();

            foreach (Process process in processList)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle) && !excludedWindows.Contains(process.GetProcessFileName()))
                {
                    windowProcesses.Add(process);
                }
            }

            return windowProcesses;
        }

        public Process GetProcess()
        {
            return selectedProcess;
        }
    }
}
