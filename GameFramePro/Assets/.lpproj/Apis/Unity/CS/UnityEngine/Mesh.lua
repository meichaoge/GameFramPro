---@class CS.UnityEngine.Mesh : CS.UnityEngine.Object
CS.UnityEngine.Mesh = {}

---@property readwrite CS.UnityEngine.Mesh.uv1 : CS.UnityEngine.Vector2[]
CS.UnityEngine.Mesh.uv1 = nil

---@property readwrite CS.UnityEngine.Mesh.indexFormat : CS.UnityEngine.Rendering.IndexFormat
CS.UnityEngine.Mesh.indexFormat = nil

---@property readonly CS.UnityEngine.Mesh.vertexBufferCount : CS.System.Int32
CS.UnityEngine.Mesh.vertexBufferCount = nil

---@property readonly CS.UnityEngine.Mesh.blendShapeCount : CS.System.Int32
CS.UnityEngine.Mesh.blendShapeCount = nil

---@property readwrite CS.UnityEngine.Mesh.bindposes : CS.UnityEngine.Matrix4x4[]
CS.UnityEngine.Mesh.bindposes = nil

---@property readonly CS.UnityEngine.Mesh.isReadable : CS.System.Boolean
CS.UnityEngine.Mesh.isReadable = nil

---@property readonly CS.UnityEngine.Mesh.vertexCount : CS.System.Int32
CS.UnityEngine.Mesh.vertexCount = nil

---@property readwrite CS.UnityEngine.Mesh.subMeshCount : CS.System.Int32
CS.UnityEngine.Mesh.subMeshCount = nil

---@property readwrite CS.UnityEngine.Mesh.bounds : CS.UnityEngine.Bounds
CS.UnityEngine.Mesh.bounds = nil

---@property readwrite CS.UnityEngine.Mesh.vertices : CS.UnityEngine.Vector3[]
CS.UnityEngine.Mesh.vertices = nil

---@property readwrite CS.UnityEngine.Mesh.normals : CS.UnityEngine.Vector3[]
CS.UnityEngine.Mesh.normals = nil

---@property readwrite CS.UnityEngine.Mesh.tangents : CS.UnityEngine.Vector4[]
CS.UnityEngine.Mesh.tangents = nil

---@property readwrite CS.UnityEngine.Mesh.uv : CS.UnityEngine.Vector2[]
CS.UnityEngine.Mesh.uv = nil

---@property readwrite CS.UnityEngine.Mesh.uv2 : CS.UnityEngine.Vector2[]
CS.UnityEngine.Mesh.uv2 = nil

---@property readwrite CS.UnityEngine.Mesh.uv3 : CS.UnityEngine.Vector2[]
CS.UnityEngine.Mesh.uv3 = nil

---@property readwrite CS.UnityEngine.Mesh.uv4 : CS.UnityEngine.Vector2[]
CS.UnityEngine.Mesh.uv4 = nil

---@property readwrite CS.UnityEngine.Mesh.uv5 : CS.UnityEngine.Vector2[]
CS.UnityEngine.Mesh.uv5 = nil

---@property readwrite CS.UnityEngine.Mesh.uv6 : CS.UnityEngine.Vector2[]
CS.UnityEngine.Mesh.uv6 = nil

---@property readwrite CS.UnityEngine.Mesh.uv7 : CS.UnityEngine.Vector2[]
CS.UnityEngine.Mesh.uv7 = nil

---@property readwrite CS.UnityEngine.Mesh.uv8 : CS.UnityEngine.Vector2[]
CS.UnityEngine.Mesh.uv8 = nil

---@property readwrite CS.UnityEngine.Mesh.colors : CS.UnityEngine.Color[]
CS.UnityEngine.Mesh.colors = nil

---@property readwrite CS.UnityEngine.Mesh.colors32 : CS.UnityEngine.Color32[]
CS.UnityEngine.Mesh.colors32 = nil

---@property readwrite CS.UnityEngine.Mesh.triangles : CS.System.Int32[]
CS.UnityEngine.Mesh.triangles = nil

---@property readwrite CS.UnityEngine.Mesh.boneWeights : CS.UnityEngine.BoneWeight[]
CS.UnityEngine.Mesh.boneWeights = nil

---@return CS.UnityEngine.Mesh
function CS.UnityEngine.Mesh()
end

---@param index : CS.System.Int32
---@return CS.System.IntPtr
function CS.UnityEngine.Mesh:GetNativeVertexBufferPtr(index)
end

---@return CS.System.IntPtr
function CS.UnityEngine.Mesh:GetNativeIndexBufferPtr()
end

function CS.UnityEngine.Mesh:ClearBlendShapes()
end

---@param shapeIndex : CS.System.Int32
---@return CS.System.String
function CS.UnityEngine.Mesh:GetBlendShapeName(shapeIndex)
end

---@param blendShapeName : CS.System.String
---@return CS.System.Int32
function CS.UnityEngine.Mesh:GetBlendShapeIndex(blendShapeName)
end

---@param shapeIndex : CS.System.Int32
---@return CS.System.Int32
function CS.UnityEngine.Mesh:GetBlendShapeFrameCount(shapeIndex)
end

---@param shapeIndex : CS.System.Int32
---@param frameIndex : CS.System.Int32
---@return CS.System.Single
function CS.UnityEngine.Mesh:GetBlendShapeFrameWeight(shapeIndex, frameIndex)
end

---@param shapeIndex : CS.System.Int32
---@param frameIndex : CS.System.Int32
---@param deltaVertices : CS.UnityEngine.Vector3[]
---@param deltaNormals : CS.UnityEngine.Vector3[]
---@param deltaTangents : CS.UnityEngine.Vector3[]
function CS.UnityEngine.Mesh:GetBlendShapeFrameVertices(shapeIndex, frameIndex, deltaVertices, deltaNormals, deltaTangents)
end

---@param shapeName : CS.System.String
---@param frameWeight : CS.System.Single
---@param deltaVertices : CS.UnityEngine.Vector3[]
---@param deltaNormals : CS.UnityEngine.Vector3[]
---@param deltaTangents : CS.UnityEngine.Vector3[]
function CS.UnityEngine.Mesh:AddBlendShapeFrame(shapeName, frameWeight, deltaVertices, deltaNormals, deltaTangents)
end

---@param bonesPerVertex : CS.Unity.Collections.NativeArray
---@param weights : CS.Unity.Collections.NativeArray
function CS.UnityEngine.Mesh:SetBoneWeights(bonesPerVertex, weights)
end

---@return CS.Unity.Collections.NativeArray
function CS.UnityEngine.Mesh:GetAllBoneWeights()
end

---@return CS.Unity.Collections.NativeArray
function CS.UnityEngine.Mesh:GetBonesPerVertex()
end

---@param uvSetIndex : CS.System.Int32
---@return CS.System.Single
function CS.UnityEngine.Mesh:GetUVDistributionMetric(uvSetIndex)
end

---@param vertices : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:GetVertices(vertices)
end

---@param inVertices : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:SetVertices(inVertices)
end

---@param normals : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:GetNormals(normals)
end

---@param inNormals : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:SetNormals(inNormals)
end

---@param tangents : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:GetTangents(tangents)
end

---@param inTangents : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:SetTangents(inTangents)
end

---@param colors : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:GetColors(colors)
end

---@param inColors : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:SetColors(inColors)
end

---@param colors : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:GetColors(colors)
end

---@param inColors : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:SetColors(inColors)
end

---@param channel : CS.System.Int32
---@param uvs : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:SetUVs(channel, uvs)
end

---@param channel : CS.System.Int32
---@param uvs : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:SetUVs(channel, uvs)
end

---@param channel : CS.System.Int32
---@param uvs : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:SetUVs(channel, uvs)
end

---@param channel : CS.System.Int32
---@param uvs : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:GetUVs(channel, uvs)
end

---@param channel : CS.System.Int32
---@param uvs : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:GetUVs(channel, uvs)
end

---@param channel : CS.System.Int32
---@param uvs : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:GetUVs(channel, uvs)
end

---@param submesh : CS.System.Int32
---@return CS.System.Int32[]
function CS.UnityEngine.Mesh:GetTriangles(submesh)
end

---@param submesh : CS.System.Int32
---@param applyBaseVertex : CS.System.Boolean
---@return CS.System.Int32[]
function CS.UnityEngine.Mesh:GetTriangles(submesh, applyBaseVertex)
end

---@param triangles : CS.System.Collections.Generic.List
---@param submesh : CS.System.Int32
function CS.UnityEngine.Mesh:GetTriangles(triangles, submesh)
end

---@param triangles : CS.System.Collections.Generic.List
---@param submesh : CS.System.Int32
---@param applyBaseVertex : CS.System.Boolean
function CS.UnityEngine.Mesh:GetTriangles(triangles, submesh, applyBaseVertex)
end

---@param submesh : CS.System.Int32
---@return CS.System.Int32[]
function CS.UnityEngine.Mesh:GetIndices(submesh)
end

---@param submesh : CS.System.Int32
---@param applyBaseVertex : CS.System.Boolean
---@return CS.System.Int32[]
function CS.UnityEngine.Mesh:GetIndices(submesh, applyBaseVertex)
end

---@param indices : CS.System.Collections.Generic.List
---@param submesh : CS.System.Int32
function CS.UnityEngine.Mesh:GetIndices(indices, submesh)
end

---@param indices : CS.System.Collections.Generic.List
---@param submesh : CS.System.Int32
---@param applyBaseVertex : CS.System.Boolean
function CS.UnityEngine.Mesh:GetIndices(indices, submesh, applyBaseVertex)
end

---@param submesh : CS.System.Int32
---@return CS.System.UInt32
function CS.UnityEngine.Mesh:GetIndexStart(submesh)
end

---@param submesh : CS.System.Int32
---@return CS.System.UInt32
function CS.UnityEngine.Mesh:GetIndexCount(submesh)
end

---@param submesh : CS.System.Int32
---@return CS.System.UInt32
function CS.UnityEngine.Mesh:GetBaseVertex(submesh)
end

---@param triangles : CS.System.Int32[]
---@param submesh : CS.System.Int32
function CS.UnityEngine.Mesh:SetTriangles(triangles, submesh)
end

---@param triangles : CS.System.Int32[]
---@param submesh : CS.System.Int32
---@param calculateBounds : CS.System.Boolean
function CS.UnityEngine.Mesh:SetTriangles(triangles, submesh, calculateBounds)
end

---@param triangles : CS.System.Int32[]
---@param submesh : CS.System.Int32
---@param calculateBounds : CS.System.Boolean
---@param baseVertex : CS.System.Int32
function CS.UnityEngine.Mesh:SetTriangles(triangles, submesh, calculateBounds, baseVertex)
end

---@param triangles : CS.System.Collections.Generic.List
---@param submesh : CS.System.Int32
function CS.UnityEngine.Mesh:SetTriangles(triangles, submesh)
end

---@param triangles : CS.System.Collections.Generic.List
---@param submesh : CS.System.Int32
---@param calculateBounds : CS.System.Boolean
function CS.UnityEngine.Mesh:SetTriangles(triangles, submesh, calculateBounds)
end

---@param triangles : CS.System.Collections.Generic.List
---@param submesh : CS.System.Int32
---@param calculateBounds : CS.System.Boolean
---@param baseVertex : CS.System.Int32
function CS.UnityEngine.Mesh:SetTriangles(triangles, submesh, calculateBounds, baseVertex)
end

---@param indices : CS.System.Int32[]
---@param topology : CS.UnityEngine.MeshTopology
---@param submesh : CS.System.Int32
function CS.UnityEngine.Mesh:SetIndices(indices, topology, submesh)
end

---@param indices : CS.System.Int32[]
---@param topology : CS.UnityEngine.MeshTopology
---@param submesh : CS.System.Int32
---@param calculateBounds : CS.System.Boolean
function CS.UnityEngine.Mesh:SetIndices(indices, topology, submesh, calculateBounds)
end

---@param indices : CS.System.Int32[]
---@param topology : CS.UnityEngine.MeshTopology
---@param submesh : CS.System.Int32
---@param calculateBounds : CS.System.Boolean
---@param baseVertex : CS.System.Int32
function CS.UnityEngine.Mesh:SetIndices(indices, topology, submesh, calculateBounds, baseVertex)
end

---@param bindposes : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:GetBindposes(bindposes)
end

---@param boneWeights : CS.System.Collections.Generic.List
function CS.UnityEngine.Mesh:GetBoneWeights(boneWeights)
end

---@param keepVertexLayout : CS.System.Boolean
function CS.UnityEngine.Mesh:Clear(keepVertexLayout)
end

function CS.UnityEngine.Mesh:Clear()
end

function CS.UnityEngine.Mesh:RecalculateBounds()
end

function CS.UnityEngine.Mesh:RecalculateNormals()
end

function CS.UnityEngine.Mesh:RecalculateTangents()
end

function CS.UnityEngine.Mesh:MarkDynamic()
end

---@param markNoLongerReadable : CS.System.Boolean
function CS.UnityEngine.Mesh:UploadMeshData(markNoLongerReadable)
end

function CS.UnityEngine.Mesh:Optimize()
end

function CS.UnityEngine.Mesh:OptimizeIndexBuffers()
end

function CS.UnityEngine.Mesh:OptimizeReorderVertexBuffer()
end

---@param submesh : CS.System.Int32
---@return CS.UnityEngine.MeshTopology
function CS.UnityEngine.Mesh:GetTopology(submesh)
end

---@param combine : CS.UnityEngine.CombineInstance[]
---@param mergeSubMeshes : CS.System.Boolean
---@param useMatrices : CS.System.Boolean
---@param hasLightmapData : CS.System.Boolean
function CS.UnityEngine.Mesh:CombineMeshes(combine, mergeSubMeshes, useMatrices, hasLightmapData)
end

---@param combine : CS.UnityEngine.CombineInstance[]
---@param mergeSubMeshes : CS.System.Boolean
---@param useMatrices : CS.System.Boolean
function CS.UnityEngine.Mesh:CombineMeshes(combine, mergeSubMeshes, useMatrices)
end

---@param combine : CS.UnityEngine.CombineInstance[]
---@param mergeSubMeshes : CS.System.Boolean
function CS.UnityEngine.Mesh:CombineMeshes(combine, mergeSubMeshes)
end

---@param combine : CS.UnityEngine.CombineInstance[]
function CS.UnityEngine.Mesh:CombineMeshes(combine)
end