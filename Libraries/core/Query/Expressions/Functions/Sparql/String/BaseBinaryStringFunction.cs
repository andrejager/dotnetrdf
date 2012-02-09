﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF.Parsing;
using VDS.RDF.Nodes;

namespace VDS.RDF.Query.Expressions.Functions.Sparql.String
{
    /// <summary>
    /// Abstract Base Class for SPARQL String Testing functions which take two arguments
    /// </summary>
    public abstract class BaseBinaryStringFunction
        : BaseBinaryExpression
    {
        /// <summary>
        /// Creates a new Base Binary SPARQL String Function
        /// </summary>
        /// <param name="stringExpr">String Expression</param>
        /// <param name="argExpr">Argument Expression</param>
        public BaseBinaryStringFunction(ISparqlExpression stringExpr, ISparqlExpression argExpr)
            : base(stringExpr, argExpr) { }

        public override IValuedNode Evaluate(SparqlEvaluationContext context, int bindingID)
        {
            INode x = this._leftExpr.Evaluate(context, bindingID);
            INode y = this._rightExpr.Evaluate(context, bindingID);

            if (x.NodeType == NodeType.Literal && y.NodeType == NodeType.Literal)
            {
                ILiteralNode stringLit = (ILiteralNode)x;
                ILiteralNode argLit = (ILiteralNode)y;

                if (IsValidArgumentPair(stringLit, argLit))
                {
                    return new BooleanNode(null, this.ValueInternal(stringLit, argLit));
                }
                else
                {
                    throw new RdfQueryException("The Literals provided as arguments to this SPARQL String function are not of valid forms (see SPARQL spec for acceptable combinations)");
                }
            }
            else
            {
                throw new RdfQueryException("Arguments to a SPARQL String function must both be Literal Nodes");
            }
        }

        /// <summary>
        /// Abstract method that child classes must implement to 
        /// </summary>
        /// <param name="stringLit"></param>
        /// <param name="argLit"></param>
        /// <returns></returns>
        protected abstract bool ValueInternal(ILiteralNode stringLit, ILiteralNode argLit);

        /// <summary>
        /// Determines whether the Arguments are valid
        /// </summary>
        /// <param name="stringLit">String Literal</param>
        /// <param name="argLit">Argument Literal</param>
        /// <returns></returns>
        protected bool IsValidArgumentPair(ILiteralNode stringLit, ILiteralNode argLit)
        {
            if (stringLit.DataType != null)
            {
                //If 1st argument has a DataType must be an xsd:string or not valid
                if (!stringLit.DataType.ToString().Equals(XmlSpecsHelper.XmlSchemaDataTypeString)) return false;

                if (argLit.DataType != null)
                {
                    //If 2nd argument also has a DataType must also be an xsd:string or not valid
                    if (!argLit.DataType.ToString().Equals(XmlSpecsHelper.XmlSchemaDataTypeString)) return false;
                    return true;
                }
                else if (argLit.Language.Equals(string.Empty))
                {
                    //If 2nd argument does not have a DataType but 1st does then 2nd argument must have no
                    //Language Tag
                    return true;
                }
                else
                {
                    //2nd argument does not have a DataType but 1st does BUT 2nd has a Language Tag so invalid
                    return false;
                }
            }
            else if (!stringLit.Language.Equals(string.Empty))
            {
                if (argLit.DataType != null)
                {
                    //If 1st argument has a Language Tag and 2nd Argument is typed then must be xsd:string
                    //to be valid
                    return argLit.DataType.ToString().Equals(XmlSpecsHelper.XmlSchemaDataTypeString);
                }
                else if (argLit.Language.Equals(string.Empty) || stringLit.Language.Equals(argLit.Language))
                {
                    //If 1st argument has a Language Tag then 2nd Argument must have same Language Tag 
                    //or no Language Tag in order to be valid
                    return true;
                }
                else
                {
                    //Otherwise Invalid
                    return false;
                }
            }
            else
            {
                if (argLit.DataType != null)
                {
                    //If 1st argument is plain literal then 2nd argument must be xsd:string if typed
                    return argLit.DataType.ToString().Equals(XmlSpecsHelper.XmlSchemaDataTypeString);
                }
                else if (argLit.Language.Equals(string.Empty))
                {
                    //If 1st argument is plain literal then 2nd literal cannot have a language tag to be valid
                    return true;
                }
                else
                {
                    //If 1st argument is plain literal and 2nd has language tag then invalid
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the Expression Type
        /// </summary>
        public override SparqlExpressionType Type
        {
            get
            {
                return SparqlExpressionType.Function;
            }
        }
    }
}