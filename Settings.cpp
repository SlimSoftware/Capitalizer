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

namespace settings {
	int selectedCapitalizeMode = 0;
	bool restoreAlwaysOnTop = true;
	wxString lastOpenedDir = "";

	void Load()
	{
		wxFileName configDir;
		wxOperatingSystemId osID = wxPlatformInfo::Get().GetOperatingSystemId();
		if (osID == wxOS_UNIX || osID == wxOS_UNIX_LINUX) {
			configDir.SetPath(wxGetHomeDir());
			configDir.AppendDir(".capitalizer");
		}
		else if (osID == wxOS_WINDOWS_NT) {
			configDir.SetPath(wxStandardPaths::Get().GetUserConfigDir());
			configDir.AppendDir("Slim Software");
			configDir.AppendDir("Capitalizer");
		}
		else {
			configDir.SetPath(wxStandardPaths::Get().GetUserConfigDir());
			configDir.AppendDir("Capitalizer");
		}

		wxString configDirPath = configDir.GetPath();
		if (!wxDirExists(configDirPath)) {
			configDir.Mkdir();
		}

		wxFileName configFile(configDir);
		configFile.SetFullName("settings.json");
		configFilePath = configFile.GetFullPath().ToStdString();

		if (!wxFileExists(configFile.GetFullPath())) {
			Save();
		}
		else {
			// Read the settings file and store the content in a json object
			ifstream i(configFilePath);
			json j;

			try {
				i >> j;

				selectedCapitalizeMode = j["selectedCapitalizeMode"];
				restoreAlwaysOnTop = j["restoreAlwaysOnTop"];
				wxString lastOpenedDirWxString = j["lastOpenedDir"].get<string>();
				lastOpenedDir = lastOpenedDirWxString;
			}
			catch (json::exception& ex) {
				// Reset the json file with the default settings
				SetDefault();
				Save();
				return;
			}
		}
	}

	void Save()
	{
		json j = {
			{"selectedCapitalizeMode", selectedCapitalizeMode},
			{"restoreAlwaysOnTop", restoreAlwaysOnTop},
			{"lastOpenedDir", lastOpenedDir.ToStdString()}
		};

		ofstream o(configFilePath);
		o << setw(4) << j << endl;
	}

	void SetDefault()
	{
		selectedCapitalizeMode = 0;
		restoreAlwaysOnTop = true;
		lastOpenedDir = "";
	}
}
