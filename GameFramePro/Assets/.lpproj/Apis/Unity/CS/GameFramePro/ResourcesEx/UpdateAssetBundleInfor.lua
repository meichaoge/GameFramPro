---@class CS.GameFramePro.ResourcesEx.UpdateAssetBundleInfor : CS.System.Object
CS.GameFramePro.ResourcesEx.UpdateAssetBundleInfor = {}

---@field public CS.GameFramePro.ResourcesEx.UpdateAssetBundleInfor.mAssetBundleAssetUpdateTagEnum : CS.GameFramePro.ResourcesEx.AssetBundleAssetUpdateTagEnum
CS.GameFramePro.ResourcesEx.UpdateAssetBundleInfor.mAssetBundleAssetUpdateTagEnum = nil

---@field public CS.GameFramePro.ResourcesEx.UpdateAssetBundleInfor.mNeedUpdateAssetName : CS.System.String
CS.GameFramePro.ResourcesEx.UpdateAssetBundleInfor.mNeedUpdateAssetName = nil

---@field public CS.GameFramePro.ResourcesEx.UpdateAssetBundleInfor.mAssetByteSize : CS.System.Int64
CS.GameFramePro.ResourcesEx.UpdateAssetBundleInfor.mAssetByteSize = nil

---@field public CS.GameFramePro.ResourcesEx.UpdateAssetBundleInfor.mAssetCRC : CS.System.UInt32
CS.GameFramePro.ResourcesEx.UpdateAssetBundleInfor.mAssetCRC = nil

---@return CS.GameFramePro.ResourcesEx.UpdateAssetBundleInfor
function CS.GameFramePro.ResourcesEx.UpdateAssetBundleInfor()
end

---@param tagEnum : CS.GameFramePro.ResourcesEx.AssetBundleAssetUpdateTagEnum
---@param assetName : CS.System.String
---@param byteSize : CS.System.Int64
---@param crc : CS.System.UInt32
---@return CS.GameFramePro.ResourcesEx.UpdateAssetBundleInfor
function CS.GameFramePro.ResourcesEx.UpdateAssetBundleInfor(tagEnum, assetName, byteSize, crc)
end