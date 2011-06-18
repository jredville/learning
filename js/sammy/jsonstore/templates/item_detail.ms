<div class="item-detail">
{{#item}}
	<div class="item-image">
		<img src="{{large_image}}" alt="{{title}}" />
	</div>
	<div class="item-info">
		<div class="item-artist">{{artist}}</div>
		<div class="item-title">{{title}}</div>
		<div class="item-price">${{price}}</div>
        <div class="item-form">
        	<form action="#/cart" method="post">
        		<input type="hidden" name="item_id" value="{{id}}" />
        		<p>
        			<label for="quantity">Quantity: </label>
        			<input type="text" size="2" name="quantity" value="1" />
        		</p>
        		<p>
        			<input type="submit" value="Add to Cart" />
        		</p>
        	</form>
        </div>
		<div class="item-link">
			<a href="{{url}}">Buy this item on Amazon</a>
		</div>
		<div class="back-link">
			<a href="#/">&laquo; Back to Items</a>
		</div>
	</div>
{{/item}}
</div>
