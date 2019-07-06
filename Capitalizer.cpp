// For compilers that support precompilation, includes "wx/wx.h".
#include <wx/wxprec.h>
#ifndef WX_PRECOMP
    #include <wx/wx.h>
#endif

#include "Capitalizer.h"
#include "MainWindow.h"
#include "icon.xpm"

bool Capitalizer::OnInit()
{
    MainWindow *mainWindow = new MainWindow("Capitalizer", wxPoint(50, 50), wxSize(800, 600));
    mainWindow->SetIcon((wxIcon)icon_xpm);
    mainWindow->Show(true);
    return true;
}
