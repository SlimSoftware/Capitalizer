// For compilers that support precompilation, includes "wx/wx.h".
#include <wx/wxprec.h>
#ifndef WX_PRECOMP
    #include <wx/wx.h>
#endif
#include "wx/xrc/xmlres.h"

#include "Capitalizer.h"
#include "MainFrame.h"
#include "icon.xpm"

bool Capitalizer::OnInit()
{
    MainFrame *frame = new MainFrame("Capitalizer", wxPoint(50, 50), wxSize(800, 600));
    frame->SetIcon((wxIcon)icon_xpm);
    frame->Show(true);
    return true;
}
