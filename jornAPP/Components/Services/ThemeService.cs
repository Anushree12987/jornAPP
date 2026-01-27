using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jornAPP.Components.Data;
using jornAPP.Components.Models;
using SQLite;


namespace jornAPP.Services
{
    /// <summary>
    /// Service to manage the application's theme (Dark/Light mode).
    /// Provides methods to toggle or set the theme and notifies subscribers when a change occurs.
    /// </summary>
    public class ThemeService
    {
        /// <summary>
        /// Indicates the current theme.
        /// True = Dark mode, False = Light mode.
        /// </summary>
        public bool IsDark { get; private set; } = true;

        /// <summary>
        /// Event triggered whenever the theme changes.
        /// UI components or pages can subscribe to update their appearance immediately.
        /// </summary>
        public event Action? OnThemeChanged;

        /// <summary>
        /// Toggles the theme between Dark and Light modes.
        /// Invokes the OnThemeChanged event to notify subscribers.
        /// </summary>
        public void ToggleTheme()
        {
            IsDark = !IsDark;          // Switch the current theme state
            OnThemeChanged?.Invoke();   // Notify all subscribers about the change
        }

        /// <summary>
        /// Sets the theme explicitly to Dark or Light mode.
        /// Invokes the OnThemeChanged event to notify subscribers.
        /// </summary>
        /// <param name="isDark">True for Dark mode, False for Light mode.</param>
        public void SetTheme(bool isDark)
        {
            IsDark = isDark;            // Set the current theme state
            OnThemeChanged?.Invoke();   // Notify all subscribers about the change
        }
    }
}