---@class CS.GameFramePro.CsvUtility : CS.Single
CS.GameFramePro.CsvUtility = {}

---@field public CS.GameFramePro.CsvUtility.CsvSeparatorChar : CS.System.Char
CS.GameFramePro.CsvUtility.CsvSeparatorChar = nil

---@field public CS.GameFramePro.CsvUtility.CsvVectorSeparatorChar : CS.System.Char
CS.GameFramePro.CsvUtility.CsvVectorSeparatorChar = nil

---@return CS.GameFramePro.CsvUtility
function CS.GameFramePro.CsvUtility()
end

---@param builder : CS.System.Text.StringBuilder
---@return CS.System.Boolean
function CS.GameFramePro.CsvUtility.WriteCsv_NewLine(builder)
end

---@param data : CS.System.String
---@param builder : CS.System.Text.StringBuilder
---@return CS.System.Boolean
function CS.GameFramePro.CsvUtility.WriteCsv(data, builder)
end

---@param data : CS.System.Boolean
---@param builder : CS.System.Text.StringBuilder
---@return CS.System.Boolean
function CS.GameFramePro.CsvUtility.WriteCsv(data, builder)
end

---@param data : CS.System.Int32
---@param builder : CS.System.Text.StringBuilder
---@return CS.System.Boolean
function CS.GameFramePro.CsvUtility.WriteCsv(data, builder)
end

---@param data : CS.System.UInt32
---@param builder : CS.System.Text.StringBuilder
---@return CS.System.Boolean
function CS.GameFramePro.CsvUtility.WriteCsv(data, builder)
end

---@param data : CS.System.Int64
---@param builder : CS.System.Text.StringBuilder
---@return CS.System.Boolean
function CS.GameFramePro.CsvUtility.WriteCsv(data, builder)
end

---@param data : CS.System.UInt64
---@param builder : CS.System.Text.StringBuilder
---@return CS.System.Boolean
function CS.GameFramePro.CsvUtility.WriteCsv(data, builder)
end

---@param data : CS.System.Single
---@param builder : CS.System.Text.StringBuilder
---@return CS.System.Boolean
function CS.GameFramePro.CsvUtility.WriteCsv(data, builder)
end

---@param data : CS.System.Double
---@param builder : CS.System.Text.StringBuilder
---@return CS.System.Boolean
function CS.GameFramePro.CsvUtility.WriteCsv(data, builder)
end

---@param data : CS.System.Decimal
---@param builder : CS.System.Text.StringBuilder
---@return CS.System.Boolean
function CS.GameFramePro.CsvUtility.WriteCsv(data, builder)
end

---@param vector : CS.UnityEngine.Vector2
---@param builder : CS.System.Text.StringBuilder
---@return CS.System.Boolean
function CS.GameFramePro.CsvUtility.WriteCsv(vector, builder)
end

---@param vector : CS.UnityEngine.Vector3
---@param builder : CS.System.Text.StringBuilder
---@return CS.System.Boolean
function CS.GameFramePro.CsvUtility.WriteCsv(vector, builder)
end

---@param csvData : CS.System.String
---@return CS.System.String
function CS.GameFramePro.CsvUtility.ReadCsv_String(csvData)
end

---@param csvData : CS.System.String
---@return CS.System.Boolean
function CS.GameFramePro.CsvUtility.ReadCsv_bool(csvData)
end

---@param csvData : CS.System.String
---@return CS.System.Int32
function CS.GameFramePro.CsvUtility.ReadCsv_Int(csvData)
end

---@param csvData : CS.System.String
---@return CS.System.UInt32
function CS.GameFramePro.CsvUtility.ReadCsv_Uint(csvData)
end

---@param csvData : CS.System.String
---@return CS.System.Int64
function CS.GameFramePro.CsvUtility.ReadCsv_Long(csvData)
end

---@param csvData : CS.System.String
---@return CS.System.UInt64
function CS.GameFramePro.CsvUtility.ReadCsv_Ulong(csvData)
end

---@param csvData : CS.System.String
---@return CS.System.Single
function CS.GameFramePro.CsvUtility.ReadCsv_Float(csvData)
end

---@param csvData : CS.System.String
---@return CS.System.Double
function CS.GameFramePro.CsvUtility.ReadCsv_Double(csvData)
end

---@param csvData : CS.System.String
---@return CS.System.Decimal
function CS.GameFramePro.CsvUtility.ReadCsv_Decimal(csvData)
end

---@param csvData : CS.System.String
---@return CS.UnityEngine.Vector2
function CS.GameFramePro.CsvUtility.ReadCsv_Vector2(csvData)
end

---@param csvData : CS.System.String
---@return CS.UnityEngine.Vector3
function CS.GameFramePro.CsvUtility.ReadCsv_Vector3(csvData)
end