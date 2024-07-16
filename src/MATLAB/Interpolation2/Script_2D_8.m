[canvas,minX,minY]=PlotLines(allSlices(DrawSlices),bestSlice);

graymap(canvas);
figure;colormap gray;
for I=1:3
quality=TestLines(allSlices(DrawSlices),bestSlice, canvas, minX, minY);

good = find(quality<1000);

[canvas,minX,minY]=PlotLines(allSlices(good),bestSlice);

imagesc(canvas);
end