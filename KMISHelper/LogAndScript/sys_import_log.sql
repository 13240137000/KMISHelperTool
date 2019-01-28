/*
 Navicat Premium Data Transfer

 Source Server         : Local MySQL 5.7
 Source Server Type    : MySQL
 Source Server Version : 50721
 Source Host           : 127.0.0.1:3306
 Source Schema         : kmis

 Target Server Type    : MySQL
 Target Server Version : 50721
 File Encoding         : 65001

 Date: 12/11/2018 16:50:50
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for sys_import_log
-- ----------------------------
DROP TABLE IF EXISTS `sys_import_log`;
CREATE TABLE `sys_import_log`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `message` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `type` int(11) NULL DEFAULT NULL,
  `module_name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `created_on_date` datetime(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `kindergarten_id` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 129 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
