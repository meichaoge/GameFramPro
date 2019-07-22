---@module CS.GameFramePro
CS.GameFramePro = {}

---@class CS.GameFramePro.IOUtility : CS.System.Object
CS.GameFramePro.IOUtility = {}

---@param filePath : CS.System.String
---@param content : CS.System.String
---@param isAppend : CS.System.Boolean
function CS.GameFramePro.IOUtility.CreateOrSetFileContent(filePath, content, isAppend)
end

---@param filePath : CS.System.String
---@param byteData : CS.System.Byte[]
---@param isAppend : CS.System.Boolean
function CS.GameFramePro.IOUtility.CreateOrSetFileContent(filePath, byteData, isAppend)
end

---@param filePath : CS.System.String
---@param content : CS.System.String
---@return CS.System.Boolean
function CS.GameFramePro.IOUtility.GetFileContent(filePath, content)
end

---@param filePath : CS.System.String
---@param newFileName : CS.System.String
---@return CS.System.Boolean
function CS.GameFramePro.IOUtility.FileModifyName(filePath, newFileName)
end

---@param filePath : CS.System.String
---@return CS.System.Boolean
function CS.GameFramePro.IOUtility.DeleteFile(filePath)
end

---@param currentPath : CS.System.String
---@param parentDeeep : CS.System.Int32
---@return CS.System.String
function CS.GameFramePro.IOUtility.GetFilePathParentDirectory(currentPath, parentDeeep)
end

---@param targetPath : CS.System.String
---@param parentDeep : CS.System.UInt32
---@return CS.System.String
function CS.GameFramePro.IOUtility.GetPathDirectoryNameByDeep(targetPath, parentDeep)
end

---@param currentPath : CS.System.String
---@param searchDirectoryName : CS.System.String
---@return CS.System.Boolean
function CS.GameFramePro.IOUtility.IsContainDirectory(currentPath, searchDirectoryName)
end

---@param directoryPath : CS.System.String
---@return CS.System.Boolean
function CS.GameFramePro.IOUtility.CheckOrCreateDirectory(directoryPath)
end

---@param directoryPath : CS.System.String
---@param isAutoCreate : CS.System.Boolean
---@return CS.System.Boolean
function CS.GameFramePro.IOUtility.ClearOrCreateDirectory(directoryPath, isAutoCreate)
end

---@param sourcePath : CS.System.String
---@param destinationPath : CS.System.String
---@param exculdeExtension : CS.System.String[]
function CS.GameFramePro.IOUtility.CopyDirectory(sourcePath, destinationPath, exculdeExtension)
end

---@param path : CS.System.String
---@param searchPattern : CS.System.String
---@param searchOption : CS.System.IO.SearchOption
---@param exculdeExtension : CS.System.String[]
---@return CS.System.String[]
function CS.GameFramePro.IOUtility.GetFilesExculde(path, searchPattern, searchOption, exculdeExtension)
end

---@param path : CS.System.String
---@param searchPattern : CS.System.String
---@param searchOption : CS.System.IO.SearchOption
---@param exculdeExtension : CS.System.String[]
---@param directoryIndex : CS.System.Int32
---@param isRelativePath : CS.System.Boolean
---@return CS.System.String[]
function CS.GameFramePro.IOUtility.GetDirectoriesAndFilesExculde(path, searchPattern, searchOption, exculdeExtension, directoryIndex, isRelativePath)
end

---@param path : CS.System.String
---@param searchPattern : CS.System.String
---@param searchOption : CS.System.IO.SearchOption
---@param exculdeExtension : CS.System.String[]
---@param isRelativePath : CS.System.Boolean
---@return CS.System.String[]
function CS.GameFramePro.IOUtility.GetDirectoriesAndFilesExculde(path, searchPattern, searchOption, exculdeExtension, isRelativePath)
end

---@param path : CS.System.String
---@param exculdeExtension : CS.System.String[]
---@return CS.System.Int32
function CS.GameFramePro.IOUtility.GetContainDirectoryExculde(path, exculdeExtension)
end

---@param targetPath : CS.System.String
---@param directoryName : CS.System.String
---@param isIncludeDirectoryName : CS.System.Boolean
---@return CS.System.String
function CS.GameFramePro.IOUtility.GetPathFromSpecialDirectoryName(targetPath, directoryName, isIncludeDirectoryName)
end

---@param path1 : CS.System.String
---@param path2 : CS.System.String
---@return CS.System.String
function CS.GameFramePro.IOUtility.CombinePathEx(path1, path2)
end

---@param path1 : CS.System.String
---@param path2 : CS.System.String
---@return CS.System.String
function CS.GameFramePro.IOUtility.ConcatPathEx(path1, path2)
end

---@param targetPath : CS.System.String
---@param extensionTag : CS.System.Char
---@return CS.System.String
function CS.GameFramePro.IOUtility.GetPathWithOutExtension(targetPath, extensionTag)
end

---@param targetPath : CS.System.String
---@return CS.System.Boolean
function CS.GameFramePro.IOUtility.IsDirectoryPath(targetPath)
end

---@param targetPath : CS.System.String
---@param isRecursive : CS.System.Boolean
---@return CS.System.String
function CS.GameFramePro.IOUtility.GetFileNameWithoutExtensionEx(targetPath, isRecursive)
end

---@param targetPath : CS.System.String
---@param otherPath : CS.System.String
---@return CS.System.Boolean
function CS.GameFramePro.IOUtility.ComparePathEx(targetPath, otherPath)
end

---@param targetPath : CS.System.String
---@return CS.System.String
function CS.GameFramePro.IOUtility.GetPathStringEx(targetPath)
end

---@param byteSize : CS.System.Int32
---@param isUptoConvert : CS.System.Boolean
---@return CS.System.String
function CS.GameFramePro.IOUtility.ByteConversionOthers(byteSize, isUptoConvert)
end