// For compilers that support precompilation, includes "wx/wx.h".
#include <wx/wxprec.h>
#ifndef WX_PRECOMP
    #include <wx/wx.h>
#endif
#include <wx/dnd.h>
#include <wx/listctrl.h>
#include <wx/artprov.h>
#include <wx/dir.h>
#include <wx/filename.h>
#include <wx/stdpaths.h>
#include <wx/version.h>

#include "Capitalizer.h"
#include "MainFrame.h"

#include "img/add.xpm"
#include "img/cross.xpm"
#include "img/tick.xpm"

#include <vector>

wxMenu *MainFrame::menuCapitalizer;
wxListView *MainFrame::toRenameList;
int selectedCapitalizeMode = 0;
wxString lastOpenedDir = "";

MainFrame::MainFrame(const wxString& title, const wxPoint& pos, const wxSize& size)
    : wxFrame(NULL, wxID_ANY, title, pos, size)
{
    menuCapitalizer = new wxMenu();
    menuCapitalizer->AppendCheckItem(MENU_ALWAYS_ON_TOP, "Always on Top");
    menuCapitalizer->AppendSeparator();
    menuCapitalizer->Append(wxID_ABOUT);
    menuCapitalizer->Append(wxID_EXIT);
    wxMenuBar *menuBar = new wxMenuBar();
    menuBar->Append(menuCapitalizer, "&Capitalizer");
    SetMenuBar(menuBar);
	
    wxToolBar *toolBar = CreateToolBar(wxTB_TEXT);
    wxBitmap addBmp(add_xpm);
    wxBitmap deleteIcon(cross_xpm);
	wxBitmap applyIcon(tick_xpm);
    toolBar->AddTool(TB_ADDFILE, "Add File(s)", addBmp, "Add File(s)"); 
    toolBar->AddTool(TB_ADDDIR, "Add Folder", addBmp, "Add Folder");
    toolBar->AddSeparator();  
    toolBar->AddTool(TB_DELETE, "Delete Selected", deleteIcon, "Delete Selected");   
    toolBar->AddTool(TB_CLEAR, "Clear", deleteIcon, "Clear"); 
    toolBar->AddSeparator();

	wxString modeChoices[] = { "Capitalize Every Word", "Like in a sentence", "all lowercase",
		"ALL UPPERCASE", "Remove extra spaces" };
    wxChoice *modeChoice = new wxChoice(toolBar, CH_MODE, wxDefaultPosition, wxSize(200, -1), 5, modeChoices);  
    modeChoice->SetSelection(0);
    toolBar->AddControl(modeChoice, "Capitalize mode");
    toolBar->AddTool(TB_CAPITALIZE, "Capitalize", applyIcon, "Capitalize");
    toolBar->Realize();

    wxBoxSizer *mainSizer = new wxBoxSizer(wxVERTICAL);
    SetSizer(mainSizer);

    toRenameList = new wxListView(this);
    toRenameList->AppendColumn("Old Name");
    toRenameList->SetColumnWidth(0, 200); 
    toRenameList->AppendColumn("New Name");
    toRenameList->SetColumnWidth(1, 200); 
    toRenameList->AppendColumn("Type");
    toRenameList->SetColumnWidth(2, 75); 
    toRenameList->AppendColumn("Path");    
    toRenameList->SetColumnWidth(3, 300); 
    toRenameList->DragAcceptFiles(true);
    toRenameList->Connect(wxEVT_DROP_FILES, wxDropFilesEventHandler(MainFrame::OnDropFiles), NULL, this);
    mainSizer->Add(toRenameList, 1, wxEXPAND);

    /* Set focus to toRenameList to prevent the first toolbar button automatically 
       being focused when launching Capitalizer */
    toRenameList->SetFocus();
}

void MainFrame::OnAlwaysOnTopChanged(wxCommandEvent& event)
{
    if (menuCapitalizer->IsChecked(MENU_ALWAYS_ON_TOP)) {
        SetWindowStyle(wxDEFAULT_FRAME_STYLE | wxSTAY_ON_TOP);
    } else {
        SetWindowStyle(wxDEFAULT_FRAME_STYLE);
    }   
}

void MainFrame::OnExit(wxCommandEvent& event)
{
    Close(true);
}

void MainFrame::OnAbout(wxCommandEvent& event)
{
    wxMessageBox("Capitalizer v1.1 (" + wxPlatformInfo::Get().GetOperatingSystemIdName() + ")\n" + 
        "Compiled using wxWidgets " + wxVERSION_NUM_DOT_STRING, "About", wxOK | wxICON_INFORMATION);
}

void MainFrame::OnAddFile(wxCommandEvent& event)
{
    wxFileDialog *openFileDialog = new wxFileDialog(this, "Choose a file to add...", 
        wxEmptyString, wxEmptyString, wxFileSelectorDefaultWildcardStr, wxFD_MULTIPLE);

    if (lastOpenedDir != "") {
        openFileDialog->SetDirectory(lastOpenedDir);
    } else {
        openFileDialog->SetDirectory(wxStandardPaths::Get().GetDocumentsDir());
    }

    if (openFileDialog->ShowModal() == wxID_OK) {
        wxArrayString paths;
        openFileDialog->GetPaths(paths);

        for (wxString path : paths) {
            wxString fileName = wxFileNameFromPath(path);
            wxString newFileName = GetNewName(fileName);

            AddToRename(fileName, newFileName, "File", path);
        }

		lastOpenedDir = openFileDialog->GetDirectory();
	}
 
	openFileDialog->Destroy();
}

void MainFrame::OnAddDir(wxCommandEvent& event)
{
    wxDirDialog *openDirDialog = new wxDirDialog(this, "Choose a folder to add..."); 

    if (lastOpenedDir != "") {
        openDirDialog->SetPath(lastOpenedDir);
    } else {
        openDirDialog->SetPath(wxStandardPaths::Get().GetDocumentsDir());
    }

    if (openDirDialog->ShowModal() == wxID_OK) {
        wxString path = openDirDialog->GetPath();
        wxString dirName = path.AfterLast(wxFILE_SEP_PATH);
        wxString newDirName = GetNewName(dirName);

        AddToRename(dirName, newDirName, "Folder", openDirDialog->GetPath());

		// Set last opened dir to parent folder of the added folder
        wxFileName dir(path);
		lastOpenedDir = dir.GetPath();
	}
 
	openDirDialog->Destroy();
}

void MainFrame::OnDropFiles(wxDropFilesEvent& event)    
{
    if (event.GetNumberOfFiles() > 0) {
        wxString* dropped = event.GetFiles();
        wxASSERT(dropped);
        wxString path;
        wxArrayString paths;

        for (int i = 0; i < event.GetNumberOfFiles(); i++) {
            path = dropped[i];
			if (wxFileExists(path) || wxDirExists(path)) {
				paths.push_back(path);
			}
        }

        for (int i = 0; i < event.GetNumberOfFiles(); i++) {
            wxString path = paths[i];
            wxString oldName;
            wxString newName;

            // Check if the path leads to a file
            if (wxFileExists(path)) {
                wxFileName fn(path);
                // Get the file name without extention
                oldName = fn.GetName();

                newName = GetNewName(oldName);
            } else {
                // Get dir name
                oldName = path.AfterLast(wxFILE_SEP_PATH);
                             
                newName = GetNewName(oldName);
            }

            wxString type;
            if (wxFileExists(path)) {
                type = "File";
            } else if (wxDirExists(path)) {
                type = "Folder";
            } else {
                type = "Unknown";
            }

            AddToRename(oldName, newName, type, path);
        }
    }
}

/**
	Adds an item to the listview
*/
void MainFrame::AddToRename(wxString oldName, wxString newName, wxString type, wxString path) 
{
    long insertIndex = toRenameList->GetItemCount();
    toRenameList->InsertItem(insertIndex, oldName);
    toRenameList->SetItem(insertIndex, 1, newName);
    toRenameList->SetItem(insertIndex, 2, type);
    toRenameList->SetItem(insertIndex, 3, path);
}

/**
	Returns the new name based on the specified name and the currently selected capitalization mode
*/
wxString MainFrame::GetNewName(wxString& oldName) 
{
    wxString newName;

    // Capitalize
    if (selectedCapitalizeMode == 0) {      
        newName = Capitalize(oldName);
    } 
    // Like in a sentence
    else if (selectedCapitalizeMode == 1) {
        newName = oldName;
        newName = newName.Capitalize();
    }
    // Lowercase    
    else if (selectedCapitalizeMode == 2) {
        newName = oldName;
        newName.LowerCase();
    }
    // Uppercase 
    else if (selectedCapitalizeMode == 3) {
        newName = oldName;
        newName.UpperCase();
    }
	// Remove double spaces
	else if (selectedCapitalizeMode == 4) {
		newName = RemoveExtraSpaces(oldName);
	}

    if (oldName.IsSameAs(newName) == true) {      
        return "(unchanged)";
    } else {
        return newName;
    }
}

wxString MainFrame::Capitalize(wxString& stringToCapitalize)
{
    wxString newString = stringToCapitalize;

    if (newString.Contains(' ')) {
        wxArrayString splitArray = wxSplit(newString, ' ');

        for (wxString& string : splitArray) {
            string = string.Capitalize();
        }
        
        newString = wxJoin(splitArray, ' ');
    } else {
        newString = stringToCapitalize.Capitalize();
    }
    
    return newString;
}

/**
	Replaces two or more spaces with one space in the specified string and returns the result
*/
wxString MainFrame::RemoveExtraSpaces(wxString& oldString) {
	wxString newString = oldString;

	while (newString.Contains("  ")) {
		newString.Replace("  ", " ");
	}

	return newString;
}

void MainFrame::OnDelete(wxCommandEvent& event)
{
    long item = toRenameList->GetFirstSelected();

    while (item != -1) {
        toRenameList->DeleteItem(item);
        item = toRenameList->GetNextSelected(item - 1);
    }
}

void MainFrame::OnClear(wxCommandEvent& event)
{
    toRenameList->DeleteAllItems();
}

void MainFrame::OnModeChoiceChange(wxCommandEvent& event) 
{
    selectedCapitalizeMode = event.GetSelection();
    // Update all names in the new name column in listview
    for (int i = 0; i < toRenameList->GetItemCount(); i++) {
        wxString oldName = toRenameList->GetItemText(i, 0);
        wxString newName = GetNewName(oldName);
        toRenameList->SetItem(i, 1, newName);
    }
}

void MainFrame::RenameAll(wxCommandEvent& event)
{   
    int filesToRename = toRenameList->GetItemCount();
    int succesfulRenamedFiles = 0;
    std::vector<int> toRemoveItemIndexes;

    for (int i = 0; i < toRenameList->GetItemCount(); i++) {
        wxString oldName = toRenameList->GetItemText(i, 0);
        wxString newName = toRenameList->GetItemText(i, 1);
        bool renameSuccesful;

        if (newName == "(unchanged)") {
            // Do not rename
            renameSuccesful = true; 
        } else {
            wxString oldPath = toRenameList->GetItemText(i, 3); 
            wxString newPath = oldPath;       
            newPath.Replace(oldName, newName);

            bool renameSuccesful = wxRenameFile(oldPath, newPath);  
        }     

        if (renameSuccesful == true) {
            succesfulRenamedFiles++;
            toRemoveItemIndexes.push_back(i);
        }
    }

    if (succesfulRenamedFiles == filesToRename) {
        wxMessageBox("Succesfully capitalized all files", "Capitalizer", wxOK | wxICON_INFORMATION);
        toRenameList->DeleteAllItems();
    } else {
        wxMessageBox("Failed to capitalize some files", "Capitalizer", wxOK | wxICON_ERROR);
        int i = 0;
        for (int& index : toRemoveItemIndexes) {       
            toRenameList->DeleteItem(index - i);
            i++;
        }
    }
}

wxBEGIN_EVENT_TABLE(MainFrame, wxFrame)
    EVT_MENU(MENU_ALWAYS_ON_TOP, MainFrame::OnAlwaysOnTopChanged)
    EVT_TOOL(TB_ADDFILE, MainFrame::OnAddFile)
    EVT_TOOL(TB_ADDDIR, MainFrame::OnAddDir)
    EVT_TOOL(TB_CAPITALIZE, MainFrame::RenameAll)
    EVT_TOOL(TB_DELETE, MainFrame::OnDelete)
    EVT_TOOL(TB_CLEAR, MainFrame::OnClear)
    EVT_CHOICE(CH_MODE, MainFrame::OnModeChoiceChange)
    EVT_MENU(wxID_EXIT, MainFrame::OnExit)
    EVT_MENU(wxID_ABOUT, MainFrame::OnAbout)
wxEND_EVENT_TABLE()
wxIMPLEMENT_APP(Capitalizer);
