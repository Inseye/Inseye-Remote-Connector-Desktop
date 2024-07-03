import argparse
from xml.etree import ElementTree
import pathlib
from xml.etree.ElementTree import Element, SubElement
PROPERTY_GROUP = "PropertyGroup"
VERSION = "Version"
parser = argparse.ArgumentParser()
parser.add_argument("-f", "--file", required=True, help="CsProj file with version to set")
parser.add_argument("-v", "--version", required=True, help="Version to set")
args = parser.parse_args()

path = pathlib.Path(args.file)
if not path.exists():
    raise Exception("Missing csproj file")
tree = ElementTree.parse(args.file)
root = tree.getroot()


def walk_property_group(property_group) -> Element | None:
    for child in property_group:
        if child.tag == PROPERTY_GROUP:
            walk_property_group(property_group)
        elif child.tag == VERSION:
            return child
    return None


for child in root:
    if child.tag == "PropertyGroup":
        node = walk_property_group(child)
        if node is not None:
            node.text = args.version
            break
else:
    prop_group = SubElement(root, PROPERTY_GROUP)
    version = SubElement(prop_group, VERSION)
    version.text = args.version
tree.write(args.file)

