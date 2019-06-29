#include <wx/wxprec.h>
#ifndef WX_PRECOMP
    #include <wx/wx.h>
#endif
#include <wx/dir.h>
#include <wx/filename.h>
#include <wx/stdpaths.h>

#include "include/json.hpp"

#include "Settings.h"

std::string configFilePath;

static int selectedCapitalizeMode;
static bool restoreAlwaysOnTop;
static wxString lastOpenedDir;

void Settings::Load() 
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

    configFile(configDir);
    configFile.SetFullName("settings.json");
    configFilePath = wxString::ToStdString(configFile.GetPath())

    if (!wxFileExists(configFile)) {
        SetDefault();
        Save();
    } else {  
        // Read the settings file and store the content in a json object
        std::ifstream i(configFilePath);
        json j;
        i >> j;
    }
}

void Settings::Save()
{
    json j = {
        {"selectedCapitalizeMode", selectedCapitalizeMode},
        {"restoreAlwaysOnTop", restoreAlwaysOnTop},
        {"lastOpenedDir", wxString::ToStdString(lastOpenedDir)}
    };

    std::ofstream o(configFilePath);
    o << std::setw(4) << j << std::endl;
}

void Settings::SetDefault()
{
    selectedCapitalizeMode = 0;
    restoreAlwaysOnTop = true;
    lastOpenedDir = "";
}