---@class CS.GameFramePro.AssetBundleInfor : CS.System.Object
CS.GameFramePro.AssetBundleInfor = {}

---@field public CS.GameFramePro.AssetBundleInfor.mBundleName : CS.System.String
CS.GameFramePro.AssetBundleInfor.mBundleName = nil

---@field public CS.GameFramePro.AssetBundleInfor.mBundleSize : CS.System.Int64
CS.GameFramePro.AssetBundleInfor.mBundleSize = nil

---@field public CS.GameFramePro.AssetBundleInfor.mBundleAssetsCount : CS.System.Int32
CS.GameFramePro.AssetBundleInfor.mBundleAssetsCount = nil

---@field public CS.GameFramePro.AssetBundleInfor.mBundleMD5Code : CS.System.String
CS.GameFramePro.AssetBundleInfor.mBundleMD5Code = nil

---@field public CS.GameFramePro.AssetBundleInfor.mBundleCRC : CS.System.UInt32
CS.GameFramePro.AssetBundleInfor.mBundleCRC = nil

---@field public CS.GameFramePro.AssetBundleInfor.mContainAssetPathInfor : CS.System.Collections.Generic.HashSet
CS.GameFramePro.AssetBundleInfor.mContainAssetPathInfor = nil

---@field public CS.GameFramePro.AssetBundleInfor.mDepdenceAssetBundleInfor : CS.System.String[]
CS.GameFramePro.AssetBundleInfor.mDepdenceAssetBundleInfor = nil

---@return CS.GameFramePro.AssetBundleInfor
function CS.GameFramePro.AssetBundleInfor()
end