import os
import GlobalVars

def LoadFunction(path, loadAs=""):
    loadType = "l"
    name = path
    src = __import__("PythonFunctions." + name, fromlist=[])
    if loadAs != "":
        name = loadAs
    if name in GlobalVars.functions:
        loadType = "rel"
    reload(src)

    components = name.split('.')
    for comp in components[:1]:
        src = getattr(src, comp)
    
    print str(src)
        
    func = src.Instantiate()

    GlobalVars.functions.update({name:func})

    return loadType

def UnloadFunction(name):
    success = True
    if name in GlobalVars.functions.keys():
        del GlobalVars.functions[name]
    else:
        success = False

    return success

def AutoLoadFunctions():
    root = os.path.join(".", "PythonFunctions")
    for item in os.listdir(root):
        if os.path.isfile(os.path.join(root, item)):
            if item.endswith(".py"):
                try:
                    LoadFunction(item[:-3])
                except Exception, x:
                    print x.message
