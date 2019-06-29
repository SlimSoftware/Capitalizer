#include <wx/wxprec.h>
#ifndef WX_PRECOMP
    #include <wx/wx.h>
#endif
#include <wx/dir.h>
#include <wx/filename.h>
#include <wx/stdpaths.h>
#include <wx/utils.h> 

#include <fstream>
#include <iomanip>

#include "include/json.hpp"

#include "Settings.h"

using json = nlohmann::json;
using namespace std;

string Settings::configFilePath;
int Settings::selectedCapitalizeMode;
bool Settings::restoreAlwaysOnTop;
wxString Settings::lastOpenedDir;

void Settings::Load() 
{    
    wxFileName configDir;
    wxOperatingSystemId osID = wxPlatformInfo::Get().GetOperatingSystemId();
    if (osID == wxOS_UNIX || osID == wxOS_UNIX_LINUX) {
        configDir.SetPath(wxGetHomeDir());
        configDir.AppendDir(".capitalizer");    
    } else {   
        configDir.SetPath(wxStandardPaths::Get().GetUserConfigDir());
        configDir.AppendDir("Capitalizer");
    }

    wxString configDirPath = configDir.GetPath();
    if (!wxDirExists(configDirPath)) {
        configDir.Mkdir();
    }

    wxFileName configFile(configDir);
    configFile.SetFullName("settings.json");
    configFilePath = configFile.GetPath().ToStdString();

    if (!wxFileExists(configFile.GetPath())) {
        SetDefault();
        Save();
    } else {  
        // Read the settings file and store the content in a json object
        ifstream i(configFilePath);
        json j;
        i >> j;
    }
}

void Settings::Save()
{
    json j = {
        {"selectedCapitalizeMode", selectedCapitalizeMode},
        {"restoreAlwaysOnTop", restoreAlwaysOnTop},
        {"lastOpenedDir", lastOpenedDir.ToStdString()}
    };

    ofstream o(configFilePath);
    o << setw(4) << j << endl;
}

void Settings::SetDefault()
{
    selectedCapitalizeMode = 0;
    restoreAlwaysOnTop = true;
    lastOpenedDir = "";
}