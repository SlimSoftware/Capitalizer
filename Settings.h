#pragma once

#include <wx/wxprec.h>
#ifndef WX_PRECOMP
    #include <wx/wx.h>
#endif
#include <wx/filename.h>

// Private namespace
namespace {
	std::string configFilePath;
}

namespace settings {
	void Load();
	void Save();
	void SetDefault();
	
	extern int selectedCapitalizeMode;
	extern bool restoreAlwaysOnTop;
	extern wxString lastOpenedDir;
}
