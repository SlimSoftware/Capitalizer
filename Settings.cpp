#include <wx/wxprec.h>
#ifndef WX_PRECOMP
    #include <wx/wx.h>
#endif
#include <wx/dir.h>
#include <wx/filename.h>
#include <wx/stdpaths.h>

#include "include/json.hpp"

static int selectedCapitalizeMode;
static bool restoreAlwaysOnTop;
static wxString lastOpenedDir;

static void Settings::Load() 
{    
    wxFileName configDir(wxStandardPaths::GetUserConfigDir());
    wxOperatingSystemId osID = wxPlatformInfo::Get().GetOperatingSystemId();
    if (osID != wxOS_UNIX) {
        configDir.AppendDir(".capitalizer");
    } else {
        configDir.AppendDir("Capitalizer")
    }

    wxString configDirPath = configDir.GetPath();
    if (!wxDirExists(configDirPath)) {
        wxDir::Make(configDirPath, 755);
    }

    wxFileName configFile(configDir);
    configFile.SetFullName("settings.json");
    if (!wxFileExists(configFile)) {
        Settings::SetDefault();
        Settings::Save();
        return;
    }
    
    // Read the settings file and save the content in a json object
    std::ifstream i(wxString::ToStdString(configFile.GetPath()));
    json j;
    i >> j;
}

static void Settings::Save()
{

}

static void Settings::SetDefault()
{
    selectedCapitalizeMode = 0;
    restoreAlwaysOnTop = true;
    lastOpenedDir = "";
}