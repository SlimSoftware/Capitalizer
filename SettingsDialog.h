#pragma once

// For compilers that support precompilation, includes "wx/wx.h".
#pragma once

#include <wx/wxprec.h>
#ifndef WX_PRECOMP
    #include <wx/wx.h>
#endif
#include <wx/listctrl.h>

class SettingsDialog: public wxDialog
{
public:
    SettingsDialog(wxWindow* parent);
	void OnClose(wxCloseEvent &event);
};
