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
	int selectedCapitalizeMode;
	bool restoreAlwaysOnTop;
	wxString lastOpenedDir;
}
