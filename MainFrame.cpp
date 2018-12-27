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
#include <wx/version.h> 
#include "Capitalizer.h"
#include "MainFrame.h"
#include "icon.xpm"

#include <vector>
#include <string>

wxListView *MainFrame::toRenameList;

MainFrame::MainFrame(const wxString& title, const wxPoint& pos, const wxSize& size)
    : wxFrame(NULL, wxID_ANY, title, pos, size)
{
    wxMenu *menuCapitalizer = new wxMenu;
    menuCapitalizer->Append(wxID_ABOUT);
    menuCapitalizer->Append(wxID_EXIT);
    wxMenuBar *menuBar = new wxMenuBar;
    menuBar->Append(menuCapitalizer, "&Capitalizer");
    SetMenuBar(menuBar);

    wxToolBar *toolBar = CreateToolBar(wxTB_TEXT);
    wxBitmap addIcon = wxArtProvider::GetBitmap(wxART_ADD_BOOKMARK);
    wxBitmap dirIcon = wxArtProvider::GetBitmap(wxART_FILE_OPEN);
    wxBitmap applyIcon = wxArtProvider::GetBitmap(wxART_TICK_MARK);
    wxBitmap deleteIcon = wxArtProvider::GetBitmap(wxART_DELETE);
    toolBar->AddTool(TB_ADDFILE, "Add File(s)", addIcon, "Add File(s)"); 
    toolBar->AddTool(TB_ADDDIR, "Add Folder", dirIcon, "Add Folder");
    toolBar->AddSeparator();  
    toolBar->AddTool(TB_DELETE, "Delete Selected", deleteIcon, "Delete Selected");   
    toolBar->AddTool(TB_CLEAR, "Clear", deleteIcon, "Clear"); 
    toolBar->AddTool(TB_CAPITALIZE, "Capitalize", applyIcon, "Capitalize");     
    toolBar->Realize();

    wxBoxSizer *mainSizer = new wxBoxSizer(wxVERTICAL);
    SetSizer(mainSizer);

    toRenameList = new wxListView(this);
    toRenameList->AppendColumn("Old Name");
    toRenameList->SetColumnWidth(0, 200); 
    toRenameList->AppendColumn("New Name");    
    toRenameList->SetColumnWidth(1, 200); 
    toRenameList->AppendColumn("Path");    
    toRenameList->SetColumnWidth(2, 400); 
    toRenameList->DragAcceptFiles(true);
    toRenameList->Connect(wxEVT_DROP_FILES, wxDropFilesEventHandler(MainFrame::OnDropFiles), NULL, this);
    mainSizer->Add(toRenameList, 1, wxEXPAND);
}

void MainFrame::OnExit(wxCommandEvent& event)
{
    Close(true);
}

void MainFrame::OnAbout(wxCommandEvent& event)
{
    wxMessageBox("Capitalizer v1.0 (" + wxPlatformInfo::Get().GetOperatingSystemIdName() + ")\n" + 
        "Compiled using wxWidgets " + wxVERSION_NUM_DOT_STRING, "About", wxOK | wxICON_INFORMATION);
}

void MainFrame::OnAddFile(wxCommandEvent& event)
{
    wxFileDialog *openFileDialog = new wxFileDialog(this, "Choose a file to add...", 
        wxEmptyString, wxEmptyString, wxFileSelectorDefaultWildcardStr, wxFD_MULTIPLE);

    if (openFileDialog->ShowModal() == wxID_OK)
	{
        wxArrayString paths;
        openFileDialog->GetPaths(paths);

        for (wxString path : paths) {
            wxString fileName = wxFileNameFromPath(path);
            wxString newFileName = Capitalize(fileName);

            long insertIndex = toRenameList->GetItemCount();
            toRenameList->InsertItem(insertIndex, fileName);
            toRenameList->SetItem(insertIndex, 1, newFileName);
            toRenameList->SetItem(insertIndex, 2, path);
        }
	}
 
	openFileDialog->Destroy();
}

void MainFrame::OnAddDir(wxCommandEvent& event)
{
    wxDirDialog *openDirDialog = new wxDirDialog(this, "Choose a folder to add..."); 
    if (openDirDialog->ShowModal() == wxID_OK)
	{
        wxDir dir(openDirDialog->GetPath());
        wxString dirName = dir.GetName();
        wxArrayString files;
        dir.GetAllFiles(openDirDialog->GetPath(), &files, wxEmptyString, wxDIR_FILES);

        for (wxString& file : files) {
            wxFileName fn = wxFileNameFromPath(file);
            // Remove the extention from the filename
            wxString fileName = fn.GetName();
            wxString newFileName = Capitalize(fileName);

            long insertIndex = toRenameList->GetItemCount();
            toRenameList->InsertItem(insertIndex, fileName);
            toRenameList->SetItem(insertIndex, 1, newFileName);
            toRenameList->SetItem(insertIndex, 2, file);
        }
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
            if (wxFileExists(path))
                paths.push_back(path);
            else if (wxDirExists(path))
                wxDir::GetAllFiles(path, &paths);                                   
        }

        for (int i = 0; i < event.GetNumberOfFiles(); i++) {
            wxString path = paths[i];
            wxFileName fn(wxFileNameFromPath(path));
            // Remove the extention from the filename
            wxString fileName = fn.GetName();
            wxString newFileName = Capitalize(fileName);

            long insertIndex = toRenameList->GetItemCount();
            toRenameList->InsertItem(insertIndex, fileName);
            toRenameList->SetItem(insertIndex, 1, newFileName);
            toRenameList->SetItem(insertIndex, 2, path);
        }
    }
}

wxString MainFrame::Capitalize(wxString stringToCapitalize)
{
    wxString newString = stringToCapitalize;

    if (newString.Contains(' '))
    {
        wxArrayString splitArray = wxSplit(newString, ' ');

        for (wxString& string : splitArray)
        {
            string = string.Capitalize();
        }
        
        newString = wxJoin(splitArray, ' ');
    } else {
        newString = stringToCapitalize.Capitalize();
    }
    
    return newString;
}

void MainFrame::OnDelete(wxCommandEvent& event)
{
    long item = -1;
    for (int i = 0; i < toRenameList->GetItemCount(); i++)
    {
        item = toRenameList->GetNextItem(item, wxLIST_NEXT_ALL, wxLIST_STATE_SELECTED);
        if ( item == -1 )
            break;
        // This item is selected, so delete it
        toRenameList->DeleteItem(item);
    }
}

void MainFrame::OnClear(wxCommandEvent& event)
{
    toRenameList->DeleteAllItems();
}

void MainFrame::RenameAll(wxCommandEvent& event)
{   
    int filesToRename = toRenameList->GetItemCount();
    int succesfulRenamedFiles = 0;
    std::vector<int> toRemoveItemIndexes;

    for (int i = 0; i < toRenameList->GetItemCount(); i++)
    {
        wxString oldName = toRenameList->GetItemText(i, 0);
        wxString newName = toRenameList->GetItemText(i, 1);
        wxString oldFilePath = toRenameList->GetItemText(i, 2); 
        wxString newFilePath = oldFilePath;       
        newFilePath.Replace(oldName, newName);

        bool renameSuccesful = wxRenameFile(oldFilePath, newFilePath);        
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
    EVT_TOOL(TB_ADDFILE, MainFrame::OnAddFile)
    EVT_TOOL(TB_ADDDIR, MainFrame::OnAddDir)
    EVT_TOOL(TB_CAPITALIZE, MainFrame::RenameAll)
    EVT_TOOL(TB_DELETE, MainFrame::OnDelete)
    EVT_TOOL(TB_CLEAR, MainFrame::OnClear)
    EVT_MENU(wxID_EXIT, MainFrame::OnExit)
    EVT_MENU(wxID_ABOUT, MainFrame::OnAbout)
wxEND_EVENT_TABLE()
wxIMPLEMENT_APP(Capitalizer);
