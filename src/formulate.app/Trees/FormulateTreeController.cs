﻿namespace formulate.app.Trees
{

    // Namespaces.
    using DataValues;
    using Folders;
    using Forms;
    using formulate.app.Helpers;
    using formulate.app.Trees.Helpers;
    using Layouts;
    using Persistence;
    using Resolvers;
    using System;
    using System.Linq;
    using System.Net.Http.Formatting;
    using Umbraco.Core;
    using Umbraco.Web.Models.Trees;
    using Umbraco.Web.Mvc;
    using Umbraco.Web.Trees;
    using Validations;
    using CoreConstants = Umbraco.Core.Constants;
    using DataSourceConstants = formulate.app.Constants.Trees.DataSources;
    using DataValueConstants = formulate.app.Constants.Trees.DataValues;
    using FormConstants = formulate.app.Constants.Trees.Forms;
    using LayoutConstants = formulate.app.Constants.Trees.Layouts;
    using ValidationConstants = formulate.app.Constants.Trees.Validations;


    //TODO: Much to do in this file.
    [Tree("formulate", "formulate", "Formulate", "icon-folder",
        "icon-folder-open", true, sortOrder: 0)]
    [PluginController("formulate")]
    public class FormulateTreeController : TreeController
    {

        private IEntityPersistence Persistence { get; set; }
        private FolderHelper FolderHelper { get; set; }
        private FormHelper FormHelper { get; set; }
        private LayoutHelper LayoutHelper { get; set; }
        private ValidationHelper ValidationHelper { get; set; }
        private DataValueHelper DataValueHelper { get; set; }


        public FormulateTreeController()
        {
            Persistence = EntityPersistence.Current.Manager;
            FolderHelper = new FolderHelper(Persistence, this);
            FormHelper = new FormHelper(this, FolderHelper);
            LayoutHelper = new LayoutHelper(this, FolderHelper);
            ValidationHelper = new ValidationHelper(this, FolderHelper);
            DataValueHelper = new DataValueHelper(this, FolderHelper);
        }

        protected override MenuItemCollection GetMenuForNode(string id,
            FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();
            var rootId = CoreConstants.System.Root.ToInvariantString();
            var rootFormsId = GuidHelper.GetGuid(FormConstants.Id);
            var rootLayoutId = GuidHelper.GetGuid(LayoutConstants.Id);
            var rootValidationId = GuidHelper.GetGuid(ValidationConstants.Id);
            var rootDataValueId = GuidHelper.GetGuid(DataValueConstants.Id);
            if (id.InvariantEquals(rootId))
            {

                // Do nothing. The root requires no actions.

            }
            else if (id.InvariantEquals(FormConstants.Id))
            {

                // Actions for forms.
                FormHelper.AddCreateFormAction(menu);
                FolderHelper.AddCreateFolderAction(menu);

            }
            else if (id.InvariantEquals(LayoutConstants.Id))
            {

                // Actions for layouts.
                LayoutHelper.AddCreateLayoutAction(menu);
                FolderHelper.AddCreateFolderAction(menu);

            }
            else if (id.InvariantEquals(DataValueConstants.Id))
            {

                // Actions for data values.
                DataValueHelper.AddCreateAction(menu);
                FolderHelper.AddCreateFolderAction(menu);

            }
            else if (id.InvariantEquals(ValidationConstants.Id))
            {

                // Actions for validations.
                ValidationHelper.AddCreateValidationAction(menu);
                FolderHelper.AddCreateFolderAction(menu);

            }
            else
            {

                // Variables.
                var entityId = GuidHelper.GetGuid(id);
                var entity = Persistence.Retrieve(entityId);
                var ancestorId = entity == null
                    ? Guid.Empty
                    : entity.Path.First();


                // What type of entity does the node represent?
                if (entity is Form)
                {
                    FormHelper.AddDeleteFormAction(menu);
                    FormHelper.AddMoveFormAction(menu, entity as Form);
                }
                else if (entity is Layout)
                {
                    LayoutHelper.AddDeleteLayoutAction(menu);
                    LayoutHelper.AddMoveLayoutAction(menu, entity as Layout);
                }
                else if (entity is Validation)
                {
                    ValidationHelper.AddDeleteValidationAction(menu);
                    ValidationHelper.AddMoveValidationAction(menu, entity as Validation);
                }
                else if (entity is DataValue)
                {
                    DataValueHelper.AddDeleteAction(menu);
                    DataValueHelper.AddMoveDataValueAction(menu, entity as DataValue);
                }
                else if (entity is Folder)
                {
                    FolderHelper.AddCreateFolderAction(menu);
                    FolderHelper.AddMoveFolderAction(menu, entity as Folder);
                    FolderHelper.AddDeleteFolderAction(menu);
                    if (ancestorId == rootFormsId)
                    {
                        FormHelper.AddCreateFormAction(menu, entityId);
                    }
                    else if (ancestorId == rootLayoutId)
                    {
                        LayoutHelper.AddCreateLayoutAction(menu, entityId);
                    }
                    else if (ancestorId == rootValidationId)
                    {
                        ValidationHelper.AddCreateValidationAction(
                            menu, entityId);
                    }
                    else if (ancestorId == rootDataValueId)
                    {
                        DataValueHelper.AddCreateAction(menu, entityId);
                    }
                }

            }
            return menu;
        }

        protected override TreeNodeCollection GetTreeNodes(string id,
            FormDataCollection queryStrings)
        {
            var entityId = GuidHelper.GetGuid(id);
            var nodes = new TreeNodeCollection();
            var rootId = CoreConstants.System.Root.ToInvariantString();
            var rootFormsId = GuidHelper.GetGuid(FormConstants.Id);
            var rootLayoutsId = GuidHelper.GetGuid(LayoutConstants.Id);
            var rootValidationsId = GuidHelper.GetGuid(
                ValidationConstants.Id);
            var rootDataValueId = GuidHelper.GetGuid(DataValueConstants.Id);
            if (id.InvariantEquals(rootId))
            {

                // Get root nodes.
                var formatUrl = "/formulate/formulate/{0}/{1}";
                var hasRootForms = Persistence
                    .RetrieveChildren(rootFormsId).Any();
                var formsNode = this.CreateTreeNode(FormConstants.Id, id,
                    queryStrings, FormConstants.Title,
                    FormConstants.TreeIcon, hasRootForms,
                    string.Format(formatUrl, "forms", FormConstants.Id));
                nodes.Add(formsNode);
                var hasRootLayouts = Persistence
                    .RetrieveChildren(rootLayoutsId).Any();
                var layoutsNode = this.CreateTreeNode(LayoutConstants.Id,
                    id, queryStrings, LayoutConstants.Title,
                    LayoutConstants.TreeIcon, hasRootLayouts,
                    string.Format(formatUrl, "layouts", LayoutConstants.Id));
                nodes.Add(layoutsNode);
                var dataSourcesNode = this.CreateTreeNode(
                    DataSourceConstants.Id, id, queryStrings,
                    DataSourceConstants.Title, DataSourceConstants.TreeIcon,
                    false, string.Format(formatUrl, "dataSources", DataSourceConstants.Id));
                nodes.Add(dataSourcesNode);
                var hasRootDataValues = Persistence
                    .RetrieveChildren(rootDataValueId).Any();
                var dataValuesNode = this.CreateTreeNode(
                    DataValueConstants.Id, id, queryStrings,
                    DataValueConstants.Title, DataValueConstants.TreeIcon,
                    hasRootDataValues,
                    string.Format(formatUrl, "dataValues", DataValueConstants.Id));
                nodes.Add(dataValuesNode);
                var hasRootValidations = Persistence
                    .RetrieveChildren(rootValidationsId).Any();
                var validationsNode = this.CreateTreeNode(
                    ValidationConstants.Id, id, queryStrings,
                    ValidationConstants.Title, ValidationConstants.TreeIcon,
                    hasRootValidations,
                    string.Format(formatUrl, "validationLibrary", ValidationConstants.Id));
                nodes.Add(validationsNode);

            }
            else if (id.InvariantEquals(LayoutConstants.Id))
            {

                // Get root nodes under layouts.
                var entities = Persistence
                    .RetrieveChildren(rootLayoutsId);
                LayoutHelper.AddLayoutChildrenToTree(nodes, queryStrings,
                    entities);

            }
            else if (id.InvariantEquals(ValidationConstants.Id))
            {

                // Get root nodes under validations.
                var entities = Persistence
                    .RetrieveChildren(rootValidationsId);
                ValidationHelper.AddValidationChildrenToTree(
                    nodes, queryStrings, entities);

            }
            else if (id.InvariantEquals(FormConstants.Id))
            {

                // Get root nodes under forms.
                var entities = Persistence
                    .RetrieveChildren(rootFormsId);
                FormHelper.AddFormChildrenToTree(nodes, queryStrings,
                    entities);

            }
            else if (id.InvariantEquals(DataValueConstants.Id))
            {

                // Get root nodes under data values.
                var entities = Persistence
                    .RetrieveChildren(rootDataValueId);
                DataValueHelper.AddChildrenToTree(nodes, queryStrings,
                    entities);

            }
            else
            {

                // Variables.
                var entity = Persistence.Retrieve(entityId);
                var ancestorId = entity.Path.First();
                var children = Persistence
                    .RetrieveChildren(entityId);


                // Add children of folder.
                if (entity is Folder)
                {

                    // Which subtree?
                    if (ancestorId == rootFormsId)
                    {
                        FormHelper.AddFormChildrenToTree(nodes, queryStrings,
                            children);
                    }
                    else if (ancestorId == rootLayoutsId)
                    {
                        LayoutHelper.AddLayoutChildrenToTree(nodes,
                            queryStrings, children);
                    }
                    else if (ancestorId == rootValidationsId)
                    {
                        ValidationHelper.AddValidationChildrenToTree(
                            nodes, queryStrings, children);
                    }
                    else if (ancestorId == rootDataValueId)
                    {
                        DataValueHelper.AddChildrenToTree(nodes,
                            queryStrings, children);
                    }

                }

            }
            return nodes;
        }


    }

}