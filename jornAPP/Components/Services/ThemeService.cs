using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jornAPP.Components.Data;
using jornAPP.Components.Models;
using SQLite;


namespace jornAPP.Services
{
    public class ThemeService
    {

        public bool IsDark { get; private set; } = true;

        public event Action? OnThemeChanged;

        public void ToggleTheme()
        {
            IsDark = !IsDark;
            OnThemeChanged?.Invoke();
        }

        public void SetTheme(bool isDark)
        {
            IsDark = isDark;
            OnThemeChanged?.Invoke();
        }
    }
}