#include <wx/wxprec.h>
#ifndef WX_PRECOMP
    #include <wx/wx.h>
#endif
#include <wx/filename.h>

class Settings 
{
    public:
    static void Load();
    static void Save();
    static void SetDefault();
    
    static int selectedCapitalizeMode;
    static bool restoreAlwaysOnTop;
    static wxString lastOpenedDir;

    private:
    std::string configFilePath;
};