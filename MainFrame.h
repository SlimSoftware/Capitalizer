#pragma once

// For compilers that support precompilation, includes "wx/wx.h".
#include <wx/wxprec.h>
#ifndef WX_PRECOMP
    #include <wx/wx.h>
#endif
#include <wx/listctrl.h>

#include "Capitalizer.h"

class MainFrame: public wxFrame
{
public:
    MainFrame(const wxString& title, const wxPoint& pos, const wxSize& size);

private:
    void OnAlwaysOnTopChanged(wxCommandEvent& event);
    void OnAddFile(wxCommandEvent& event);
    void OnAddDir(wxCommandEvent& event);
    void OnDropFiles(wxDropFilesEvent& event);
    void AddToRename(wxString oldName, wxString newName, wxString type, wxString path);
    void OnExit(wxCommandEvent& event);
    void OnAbout(wxCommandEvent& event);
    wxString GetNewName(wxString& oldName);
    wxString Capitalize(wxString& stringToCapitalize);
	wxString RemoveExtraSpaces(wxString & oldString);
	wxString UnderscoresToSpaces(wxString & oldString);
    void OnDelete(wxCommandEvent& event);
    void OnClear(wxCommandEvent& event);
    void OnModeChoiceChange(wxCommandEvent& event);
    void RenameAll(wxCommandEvent& event);
    wxDECLARE_EVENT_TABLE();

    static wxMenu *menuCapitalizer;
    static wxListView *toRenameList;
};

enum
{
    TB_ADDFILE = wxID_HIGHEST + 1,
    TB_ADDFILES,
    TB_ADDDIR,
    TB_CAPITALIZE,
    TB_DELETE,
    TB_CLEAR,
    CH_MODE,
    MENU_ALWAYS_ON_TOP
};
