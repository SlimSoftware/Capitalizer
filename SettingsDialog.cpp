// For compilers that support precompilation, includes "wx/wx.h".
#include <wx/wxprec.h>
#ifndef WX_PRECOMP
    #include <wx/wx.h>
#endif

#include "SettingsDialog.h"
#include "Settings.h"

wxCheckBox *restoreAlwaysOnTopCheckBox;

SettingsDialog::SettingsDialog(wxWindow* parent) 
	: wxDialog(parent, wxID_ANY, "Settings", wxDefaultPosition, wxSize(500, 300))
{
	Connect(wxEVT_CLOSE_WINDOW, wxCloseEventHandler(SettingsDialog::OnClose), NULL, this);
	SetBackgroundColour(*wxWHITE);

	wxBoxSizer *mainSizer = new wxBoxSizer(wxVERTICAL);
	SetSizer(mainSizer);
	restoreAlwaysOnTopCheckBox = new wxCheckBox(this, wxID_ANY, "Remember always on top state");
	wxStaticText *folderAddBehaviourLabel = new wxStaticText(this, wxID_ANY, "When a folder is added, Capitalizer should:");
	wxRadioButton *addFolderOnlyRadioBox = new wxRadioButton(this, wxID_ANY, "Add the folder only");
	wxRadioButton *addFolderAndContentsRadioBox = new wxRadioButton(this, wxID_ANY, "Add the folder and its contents");
	wxBoxSizer *folderAndContentsSizer = new wxBoxSizer(wxHORIZONTAL);
	wxTextCtrl *folderDepthTextArea = new wxTextCtrl(this, wxID_ANY, "", wxDefaultPosition, wxSize(20, 20));
	wxStaticText *folderDepthLevelLabel = new wxStaticText(this, wxID_ANY, "levels deep");

	mainSizer->Add(restoreAlwaysOnTopCheckBox, 0, wxLEFT | wxTOP, 10);
	mainSizer->AddSpacer(10);
	mainSizer->Add(folderAddBehaviourLabel, 0, wxLEFT | wxTOP, 10);
	mainSizer->Add(addFolderOnlyRadioBox, 0, wxLEFT | wxTOP, 10);
	mainSizer->AddSpacer(5);
	folderAndContentsSizer->Add(addFolderAndContentsRadioBox, 0, wxLEFT, 10);
	folderAndContentsSizer->Add(folderDepthTextArea, 0, wxLEFT, 0);
	folderAndContentsSizer->Add(folderDepthLevelLabel, 0, wxLEFT, 5);
	mainSizer->Add(folderAndContentsSizer);

	if (settings::restoreAlwaysOnTop == true) {
		restoreAlwaysOnTopCheckBox->SetValue(true);
	}
	   
	ShowModal();
}

void SettingsDialog::OnClose(wxCloseEvent& event) 
{
	if (restoreAlwaysOnTopCheckBox->GetValue() != settings::restoreAlwaysOnTop) {
		settings::restoreAlwaysOnTop = restoreAlwaysOnTopCheckBox->GetValue();
	}

	settings::Save();
	EndModal(true);
}
