using System.Text;

var basePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\";

Console.WriteLine("感谢您使用《云易工作室》出品的网易模组初始化小工具，下方的输入请全部使用英文。");

Console.Write("模组名称：");
var name = Console.ReadLine();

Console.Write("模组版本：");
var version = Console.ReadLine();

Console.Write("团队名称：");
var teamName = Console.ReadLine();

var builder = new StringBuilder();
var previousUpper = false;

for (var i = 0; i < name.Length; i++)
{
    var c = name[i];
    if (char.IsUpper(c))
    {
        if (i > 0 && !previousUpper)
        {
            builder.Append("_");
        }
        builder.Append(char.ToLowerInvariant(c));
        previousUpper = true;
    }
    else
    {
        builder.Append(c);
        previousUpper = false;
    }
}

var nameSneak = builder.ToString();

var modName = name + "Mod";

var basePackageName = teamName + name + "Scripts";


// ----- scriptBaePath -----

var scriptBaePath = basePath + basePackageName;
Directory.CreateDirectory(scriptBaePath);

// ----- scriptBaePath __init__.py -----

var path = scriptBaePath + @"\" + "__init__.py";
File.Create(path).Close();

// ----- scriptBaePath modMain.py -----

path = scriptBaePath + @"\" + "modMain.py";
File.Create(path).Close();

var modMainContent = @"# -*- coding: utf-8 -*-

import mod.client.extraClientApi as clientApi
import mod.server.extraServerApi as serverApi
from mod.common.mod import Mod

from {0}.config import modConfig


@Mod.Binding(modConfig.ModName, modConfig.ModVersion)
class {1}(object):

    @Mod.InitClient()
    def init_client(self):
        clientApi.RegisterSystem(
            modConfig.ModName,
            modConfig.ClientSystemName,
            modConfig.ClientSystemClsPath
        )

    @Mod.InitServer()
    def init_server(self):
        serverApi.RegisterSystem(
            modConfig.ModName,
            modConfig.ServerSystemName,
            modConfig.ServerSystemClsPath
        )
";

File.AppendAllText(path, String.Format(modMainContent, basePackageName, modName));


// ----- config -----

path = scriptBaePath + @"\" + "config";
Directory.CreateDirectory(path);

// ----- config __init__.py -----

path = scriptBaePath + @"\" + "config" + @"\" + "__init__.py";
File.Create(path).Close();

// ----- config modConfig.py -----

path = scriptBaePath + @"\" + "config" + @"\" + "modConfig.py";
File.Create(path).Close();

var modConfigContent = @"# -*- coding: utf-8 -*-

ModName = ""{0}""

ModVersion = ""{1}""

ClientSystemName = ""{2}ClientSystem""

ClientSystemClsPath = ""{3}.client.modClient.{2}ClientSystem""

ServerSystemName = ""{2}ServerSystem""

ServerSystemClsPath = ""{3}.server.modServer.{2}ServerSystem""

{2}RequestEvent = ""{2}RequestEvent""

{2}ResponseEvent = ""{2}ResponseEvent""
";

File.AppendAllText(path, String.Format(modConfigContent, modName, version, name, basePackageName));


// ----- client -----

path = scriptBaePath + @"\" + "client";
Directory.CreateDirectory(path);

// ----- client __init__.py -----

path = scriptBaePath + @"\" + "client" + @"\" + "__init__.py";
File.Create(path).Close();

// ----- client modClient.py -----

path = scriptBaePath + @"\" + "client" + @"\" + "modClient.py";
File.Create(path).Close();

var modClientContent = @"# -*- coding: utf-8 -*-

import mod.client.extraClientApi as clientApi

from {0}.config import modConfig

ClientSystemCls = clientApi.GetClientSystemCls()


class {1}ClientSystem(ClientSystemCls, object):

    def __init__(self, namespace, system_name):
        super({1}ClientSystem, self).__init__(namespace, system_name)
        self.listener()

    def listener(self):
        self.ListenForEvent(
            modConfig.ModName,
            modConfig.ServerSystemName,
            modConfig.{1}ResponseEvent,
            self,
            self.{2}_response_event
        )

    # 集中处理服务器响应事件    
    def {2}_response_event(self, args):
        pass
";

File.AppendAllText(path, String.Format(modClientContent, basePackageName, name, nameSneak));


// ----- server -----

path = scriptBaePath + @"\" + "server";
Directory.CreateDirectory(path);

// ----- server __init__.py -----

path = scriptBaePath + @"\" + "server" + @"\" + "__init__.py";
File.Create(path).Close();

// ----- server modServer.py -----

path = scriptBaePath + @"\" + "server" + @"\" + "modServer.py";
File.Create(path).Close();

var modServerContent = @"# -*- coding: utf-8 -*-

import mod.server.extraServerApi as serverApi

from {0}.config import modConfig

ServerSystemCls = serverApi.GetServerSystemCls()


class {1}ServerSystem(ServerSystemCls, object):

    def __init__(self, namespace, system_name):
        super({1}ServerSystem, self).__init__(namespace, system_name)
        self.listener()

    def listener(self):
        self.ListenForEvent(
            modConfig.ModName,
            modConfig.ClientSystemName,
            modConfig.{1}RequestEvent,
            self,
            self.{2}_request_event
        )

    # 集中处理客户端请求事件  
    def {2}_request_event(self, args):
        pass
";

File.AppendAllText(path, String.Format(modServerContent, basePackageName, name, nameSneak));