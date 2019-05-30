# NEO-CLI-NEL:
[English](#en) [简体中文](#zh)

<a name="zh"></a>

## 概述:

NEL团队在开发项目的过程中会遇到一些特定的需求，NEO-CLI并不能直接满足。因此修改了NEO-CLI的部分代码，加入了一些定制的功能，推出了NEL的定制版节点NEO-CLI-NEL。NEL定制版的节点包含了原NEO节点的所有功能。并保证安全性和原NEO节点一样。
注：这些需求包含但不仅限于`数据直接录入Mongodb`，`合约操作过程的记录`，`通过leveldb操作来快速恢复数据`。

## 环境要求和如何使用:

NEO-CLI-NEL的环境要求以及使用方法和原NEO-CLI一样，这里就不重复介绍。可以查阅https://github.com/neo-project/neo-cli
如果需要使用NEL特有的一些功能，可以参阅下面的定制版功能介绍。

## NEL-CLI-NEL定制功能介绍：

### 基于leveldb操作记录的快速同步
#### 简介：
去除节点在初次同步时对数据的验证分类，直接通过leveldb操作来快速同步数据。
这个功能以插件的形式实现。
具体可以查阅https://github.com/NewEconoLab/Plugins 工程中的RestoreDB项目。
#### 注意项：
与原版的插件不一样，RestoreDB这个插件需要放在NEL_Plugins目录下。
#### 使用流程：
 - 克隆本仓库；
 - 发布这个工程；
 - 创建*NEL_Plugins*文件夹（路径类似于/neo-cli/bin/debug/netcoreapp2.1/NEL_Plugins）
 - 克隆Plugins仓库（https://github.com/NewEconoLab/Plugins ）
 - 发布*RestoreDB*工程
 - 将发布RestoreDB工程生成的.dll文件以及包含配置文件的文件夹复制到NEL_Plugins文件夹中（此时NEL_Plugins文件夹中应该有Restore.dll以及一个包含config.json文件的Restore文件夹）
 - 回到neo-cli.dll所在目录，下载同步数据包 
    - testnet： http://nel-acc.oss-cn-hangzhou.aliyuncs.com/release.0-2400000.zip
    		http://nel-acc.oss-cn-hangzhou.aliyuncs.com/release.2400001-2508815.zip
    - mainnet: 暂无
 - 使用dotnet neo.cli 命令启动节点，如果出现"正在导入0-1000000高度数据"则意味着正在快速恢复数据。
    - 如果需要额外的启动项，例如--rpc --log等，使用方法和原版节点一样。
 - 等待直到出现NEO-CLI Version的提示，数据已经同步完成，节点会继续从p2p网络中获取剩余高度的数据。
 
### 节点直接将数据录入mongodb
#### 简介：
节点提供了获取block和applicationLog的接口，但是轮询请求获取数据消耗太大且有一定的延迟性。因此通过插件将数据从节点直接录入mongodb。
具体可以查阅https://github.com/NewEconoLab/Plugins 工程中的RecordToMongo项目。
#### 注意项：
与原版的插件不一样，RecordToMongo这个插件需要放在NEL_Plugins目录下。
#### 使用流程（如果已经操作过前四步，则跳过，不需要重复）：
 - 克隆本仓库；
 - 发布这个工程；
 - 创建*NEL_Plugins*文件夹（路径类似于/neo-cli/bin/debug/netcoreapp2.1/NEL_Plugins）
 - 克隆Plugins仓库（https://github.com/NewEconoLab/Plugins ）
 - 发布*RecordToMongo*工程
 - 将发布RecordToMongo工程生成的.dll文件以及包含配置文件的文件夹复制到NEL_Plugins文件夹中（此时NEL_Plugins文件夹中应该有RecordToMongo.dll以及一个包含config.json文件的RecordToMongo文件夹）
 - 按需编辑RecordToMongo文件夹下面的config.json文件。
 - 回到neo-cli.dll所在目录，使用dotnet neo-cli.dll 命令启动节点。
    - 如果需要额外的启动项，例如--rpc --log等，使用方法和原版节点一样。
#### 配置文件介绍
配置文件内容如下：
```json
{
  "PluginConfiguration": {
    "Conn": "",
    "DataBase": "",
    "Coll_Operation": "",
    "Coll_Operation_Nep5": "",
    "Coll_DumpInfo": "",
    "Coll_Block": "",
    "Coll_Application": "",
    "Coll_SystemCounter": "",
    "Coll_Nep5State": "",
    "MongoDbIndexs": [
    ]
  }
}
```
> 
> Conn : MongoDb的链接地址，必须要填；
> 
> DataBase : MongoDb的仓库名，必须要填；
> 
> Coll_Operation : 填写一个表名，意味着将节点leveldb的操作录入到这张表中。填写则代表会记录，不填写代表不记录；
> 
> Coll_Operation_Nep5 : 填写一个表名，意味着将节点leveldb关于nep5相关操作录入到这张表中。填写则代表会记录，不填写代表不记录；
> 
> Coll_DumpInfo : 填写一个表名，意味着将节点执行智能合约的过程录入到这张表中。填写则代表会记录，不填写代表不记录；
> 
> Coll_Block : 填写一个表名，意味着将节点中所有块（block）的数据都录入到这张表中，填写则代表会记录，不填写代表不记录；
> 
> Coll_Application : 填写一个表名，意味着将节点所有合约日志（ApplicationLog）的数据都录入到这张表中，填写则代表会记录，不填写代表不记录；
> 
> Coll_SystemCounter : 填写一个表名，这个表中记录了已经录入的Block的高度以及ApplicationLog的高度，如果Coll_Block或Coll_Application中有任意一个填写了，那么这个也需要填写；
> 
> Coll_Nep5State : 填写一个表名，意味着将节点记录的nep5资产的余额都录入到这张表。填写则代表会记录，不填写代表不记录；
> 
> MongoDbIndexs : 用来填写mongodb的索引。
	格式类似于："{\"collName\": \"dumpinfos\",\"indexs\": [{\"indexName\": \"i_txid_unique\",\"indexDefinition\": {\"txid\": 1},\"isUnique\": true}]}",
      "{\"collName\": \"block\",\"indexs\": [{\"indexName\": \"i_index_-1_unique\",\"indexDefinition\": {\"index\": -1},\"isUnique\": true}]}"；
> 

### 节点执行智能合约过程的记录（dumpinfo）
#### 简介：
由于智能合约是在节点底层的虚拟机中运行，外界不知道执行的具体情况，如果出错了也无法得知是在哪一个操作报的错误。这对智能合约的编写开发非常不便。
因此添加了能输出合约执行过程的这一功能。
#### 注意项：
NEO-CLI-NEL默认包含这一功能，config.json文件中有一个字段是DumpOnlyLocal，填写true代表着只会记录本节点发送的，填写false则意味着会记录链上所有的交易触发的合约过程。
如果需要将dumpinfo保存，则必须依赖插件RecordToMongo。

--------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------

<a name="en"></a>

## Overview:

The NEL team encounter some specific requirements during the development of the project, which NEO-CLI cannot directly satisfy. Therefore, part of the NEO-CLI code was modified, and some custom functions were added, and the NEL custom version node NEO-CLI-NEL was introduced. The NEL custom version of the node contains all the features of the original NEO node and ensures the same security as the original one.
Note: These requirements include, but are not limited to` direct entry of data into MongoDB``a record of contract operations``and fast recovery of data through LevelDB operations`.
Environmental requirements and how to use it.

## Environmental requirements and how to use it:

The environmental requirements and usage of NEO-CLI-NEL are the same as those of the original NEO-CLI, so we will not repeat them here. You can check out https://github.com/neo-project/neo-cli. 
If you need to use some features specific to NEL, you can refer to the custom features below.

## NEL-CLI-NEL custom features：

### Fast synchronization of LevelDB-based operation records
#### Introduction:
Remove the verification and classification of the data when the node is first synchronized and the data is directly synchronized by the LevelDB operation. 
This feature is implemented in the form of a plugin.
See the RestoreDB project in the https://github.com/NewEconoLab/Plugins project for details.
#### Note:
Unlike the original plugin, the RestoreDB plugin needs to be placed in the NEL_Plugins directory.
#### How to use it：
 - Clone this repository
 - Publish this project
 - Create a *NEL_Plugins* folder (the path is similar to /neo-cli/bin/debug/netcoreapp2.1/NEL_Plugins)
 -  (the path is similar to /neo-cli/bin/debug/netcoreapp2.1/NEL_Plugins)
 - Publish the *RestoreDB* project
 - Copy the .dll file generated by the RestoreDB project publishing and the folder containing the configuration file to the NEL_Plugins folder (there should be Restore.dll and a Restore folder containing the config.json file in the NEL_Plugins folder)
 - Go back to the directory where neo-cli.dll is located and download the sync packet
    - testnet： http://nel-acc.oss-cn-hangzhou.aliyuncs.com/release.0-2400000.zip
    		http://nel-acc.oss-cn-hangzhou.aliyuncs.com/release.2400001-2508815.zip
    - mainnet: no
 - Start the node with the dotnet neo.cli command, and if "Importing 0-000000 height data" appears, it means that data is being recovered quickly
    - If additional starting items are required, such as --rpc --log, etc., the usage is as same as the original node
 - Wait until the NEO-CLI Version prompt appears, the data has been synchronized, and the node will continue to retrieve the remaining height data from the p2p network
 
### The node directly enters data into mongodb
#### Introduction：
The node provides an interface to get the block and applicationLog, but the polling request to get the data is of low performance and has some delay. Therefore, the data is directly recorded into the mongodb from the node through the plugin. See the RecordToMongo project in the https://github.com/NewEconoLab/Plugins project for details.
#### Note：
Unlike the original plugin, the RecordToMongo plugin needs to be placed in the NEL_Plugins directory.
#### How to use it (if you have already done the first four steps, skip it, don't need to repeat it)：
 - Clone the repository
 - Publish this project
 - Create a *NEL_Plugins* folder (the path is similar to /neo-cli/bin/debug/netcoreapp2.1/NEL_Plugins)
 - Clone the Plugins repository (https://github.com/NewEconoLab/Plugins)
 - Publish RecordToMongo project
 - Copy the .dll file generated by the RecordToMongo project publishing and the folder containing the configuration file to the NEL_Plugins folder (there should be RecordToMongo.dll in the NEL_Plugins folder and a RecordToMongo folder containing the config.json file)
 - Edit the config.json file under the RecordToMongo folder as needed
 - Go back to the directory where neo-cli.dll is located and start the node with the dotnet neo-cli.dll command
    - If additional starting items are required, such as --rpc --log, etc., the same method as the original node
#### Introduction of the configuration file
The contents of the configuration file are as follows：
```json
{
  "PluginConfiguration": {
    "Conn": "",
    "DataBase": "",
    "Coll_Operation": "",
    "Coll_Operation_Nep5": "",
    "Coll_DumpInfo": "",
    "Coll_Block": "",
    "Coll_Application": "",
    "Coll_SystemCounter": "",
    "Coll_Nep5State": "",
    "MongoDbIndexs": [
    ]
  }
}
```
> 
> Conn : The link address of MongoDb, which must be filled in.
> 
> DataBase : The name of the MongoDb repository, which must be filled in.
> 
> Coll_Operation : Fill in a table name, which means that the operation of the node leveldb is entered into this table. Filling in means recording, not filling in means not recording.
> 
> Coll_Operation_Nep5 : Fill in a table name, which means that the node leveldb records the nep5 related operations into this table. Filling in means recording, not filling in means not recording.
> 
> Coll_DumpInfo : Fill in a table name, which means that the process of executing the smart contract by the node is entered into this table. Filling in means recording, not filling in means not recording.
> 
> Coll_Block : Fill in a table name, which means that all the data of the block in the node will be entered into this table. Filling in means recording, not filling in means not recording.
> 
> Coll_Application : Fill in a table name, which means that all the contract log (ApplicationLog) data of the node will be entered into this table. Filling in means recording, not filling in means not recording.
> 
> Coll_SystemCounter : Fill in a table name, this table records the height of the Block that has been entered and the height of the ApplicationLog. If any of the Coll_Block or Coll_Application is filled in, then this also needs to be filled out.
> 
> Coll_Nep5State : Fill in a table name, which means that the balance of the nep5 assets recorded by the node is entered into this table. Filling in means recording, not filling in means not recording.
> 
> MongoDbIndexs : Used to fill in the index of mongodb
	 The format is similar to："{\"collName\": \"dumpinfos\",\"indexs\": [{\"indexName\": \"i_txid_unique\",\"indexDefinition\": {\"txid\": 1},\"isUnique\": true}]}",
      "{\"collName\": \"block\",\"indexs\": [{\"indexName\": \"i_index_-1_unique\",\"indexDefinition\": {\"index\": -1},\"isUnique\": true}]}"
> 

### The process record of node performing the smart contract (dumpinfo)
#### Introduction：
Since the smart contract is run in the virtual machine at the bottom of the node, the outside world does not know the specific situation of the execution. If an error occurs, it is impossible to know which operation is reported to be incorrect. This is very inconvenient for the development of smart contracts. So this feature has been added to output the contract execution process.
#### Note：
NEO-CLI-NEL includes this function by default. One field in the config.json file is DumpOnlyLocal. Filling in true means that only the transaction sent by this node will be recorded.
If you fill in false, it will record the contract process triggered by all transactions on the chain. If you need to save dumpinfo, you must rely on the plugin RecordToMongo.
