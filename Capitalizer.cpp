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
    MainWindow *frame = new MainWindow("Capitalizer", wxPoint(50, 50), wxSize(800, 600));
    frame->SetIcon((wxIcon)icon_xpm);
    frame->Show(true);
    return true;
}
