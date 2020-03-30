import { CreateNodeArgs, PluginOptions } from "gatsby";
import { createRemoteFileNode } from "gatsby-source-filesystem";

export class MediaNodeBuilder {

  constructor(private args: CreateNodeArgs, private options: PluginOptions) {
  }

  async build() {

    const { node, cache, store, createNodeId, reporter, actions: { createNode } } = this.args;

    if (node.internal.type === "AllorsMedia") {

      const fileName = `${node.fileName}`.replace(/\s+/g, '').replace(/[^a-zA-Z0-9.]+/g, '_');
      const rawUrl = `${this.options["url"]}media/${node.uniqueId}/${fileName}`;
      const url = encodeURI(rawUrl);

      try {
        const fileNode = await createRemoteFileNode({
          url: url,
          parentNodeId: node.id,
          store,
          cache,
          createNode,
          createNodeId,
          reporter
        });

        node.file___NODE = fileNode.id;
      } catch (e) {
        reporter.panicOnBuild("Could not create remote file node for " + url, e);
      }
    }
  }
}